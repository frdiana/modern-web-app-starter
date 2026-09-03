using Microsoft.AspNetCore.Http.HttpResults;
using VerticalSlice.Api.Authentication;
using VerticalSlice.Api.Features;
using VerticalSlice.Api.Filters;
using VerticalSlice.Domain;

namespace VerticalSlice.Api.Endpoints.Examples;

public sealed class ListGreetings : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/examples/greetings", Handle)
            .WithName("ListGreetings")
            .WithSummary("List greetings")
            .WithDescription("Returns greetings from the configured persistence provider.")
            .WithTags("Examples")
            .Produces<Response[]>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .AddEndpointFilter(new FeatureFlagFilter(FeatureFlags.GreetingEndpoint))
            .RequireRateLimiting(RateLimitingPolicies.Api)
            .RequireConfiguredAuthorization();
    }

    private static async Task<Ok<Response[]>> Handle(
        IGreetingRepository repository,
        CancellationToken cancellationToken)
    {
        var greetings = await repository.ListAsync(cancellationToken);
        var response = greetings
            .Select(static greeting => new Response(
                greeting.Id,
                greeting.Message,
                greeting.CreatedAt))
            .ToArray();

        return TypedResults.Ok(response);
    }

    public sealed record Response(Guid Id, string Message, DateTimeOffset CreatedAt);
}