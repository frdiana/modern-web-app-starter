using Microsoft.FeatureManagement;

namespace ModernWebApp.Api.Filters;

internal sealed class FeatureFlagFilter(string featureName) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var featureManager = context.HttpContext.RequestServices
            .GetRequiredService<IFeatureManagerSnapshot>();

        return await featureManager.IsEnabledAsync(featureName)
            ? await next(context)
            : TypedResults.NotFound();
    }
}