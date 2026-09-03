using Aspire.Hosting.ApplicationModel;

internal static class AuthenticationExtensions
{
    public static IResourceBuilder<ExecutableResource> AddFrontendAuthentication(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ExecutableResource> frontend)
    {
        _ = builder;
        return frontend;
    }
}