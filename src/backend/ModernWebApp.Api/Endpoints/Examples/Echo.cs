using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using ModernWebApp.Api.Configuration;
using ModernWebApp.Api.Extensions;
using ModernWebApp.Api.Features;
using ModernWebApp.Api.Filters;
using ModernWebApp.Domain;

namespace ModernWebApp.Api.Endpoints.Examples;

public sealed class Echo : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/examples/echo", Handle)
            .AddEndpointFilter<ValidationFilter<Request>>()
            .WithName("Echo")
            .WithSummary("Echo a message")
            .WithDescription("Returns the supplied message with the current UTC time.")
            .WithTags("Examples")
            .Produces<Response>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .AddEndpointFilter(new FeatureFlagFilter(FeatureFlags.GreetingEndpoint))
            .RequireRateLimiting(RateLimitingPolicies.Api)
            .RequireConfiguredAuthorization();
    }

    private static Ok<Response> Handle(
        [AsParameters] Request request,
        IClock clock,
        IOptions<ApplicationOptions> applicationOptions,
        ILogger<Echo> logger)
    {
        logger.LogInformation(
            "Greeting requested for application {ApplicationName}",
            applicationOptions.Value.Name);

        return TypedResults.Ok(new Response(
            request.Message!,
            applicationOptions.Value.Name,
            clock.UtcNow));
    }

    public sealed record Request
    {
        public string? Message { get; init; }
    }

    public sealed record Response(
        string Message,
        string ApplicationName,
        DateTimeOffset Timestamp);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(static request => request.Message)
                .NotEmpty()
                .MaximumLength(200);
        }
    }
}