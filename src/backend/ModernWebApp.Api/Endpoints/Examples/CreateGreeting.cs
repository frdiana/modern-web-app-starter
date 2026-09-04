using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using ModernWebApp.Api.Authentication;
using ModernWebApp.Api.Features;
using ModernWebApp.Api.Filters;
using ModernWebApp.Domain;

namespace ModernWebApp.Api.Endpoints.Examples;

public sealed class CreateGreeting : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/examples/greetings", Handle)
            .AddEndpointFilter<ValidationFilter<Request>>()
            .WithName("CreateGreeting")
            .WithSummary("Create a greeting")
            .WithDescription("Stores a greeting using the configured persistence provider.")
            .WithTags("Examples")
            .Produces<Response>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .AddEndpointFilter(new FeatureFlagFilter(FeatureFlags.GreetingEndpoint))
            .RequireRateLimiting(RateLimitingPolicies.Api)
            .RequireConfiguredAuthorization();
    }

    private static async Task<Created<Response>> Handle(
        Request request,
        IGreetingRepository repository,
        IClock clock,
        CancellationToken cancellationToken)
    {
        var greeting = new Greeting(Guid.NewGuid(), request.Message.Trim(), clock.UtcNow);
        await repository.AddAsync(greeting, cancellationToken);
        var response = new Response(greeting.Id, greeting.Message, greeting.CreatedAt);

        return TypedResults.Created($"/api/examples/greetings/{greeting.Id}", response);
    }

    public sealed record Request(string Message);

    public sealed record Response(Guid Id, string Message, DateTimeOffset CreatedAt);

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