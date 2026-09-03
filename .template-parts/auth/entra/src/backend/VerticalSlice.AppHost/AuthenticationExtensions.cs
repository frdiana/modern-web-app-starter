using Aspire.Hosting.ApplicationModel;

internal static class AuthenticationExtensions
{
    public static IResourceBuilder<ExecutableResource> AddFrontendAuthentication(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ExecutableResource> frontend)
    {
        _ = builder;
        return frontend
            .WithHttpEndpoint(port: 5173, name: "http")
            .WithEnvironment(
                "VITE_ENTRA_REDIRECT_URI",
                "http://localhost:5173");
    }
}