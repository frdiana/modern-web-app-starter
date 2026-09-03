using Aspire.Hosting.ApplicationModel;

internal static class PersistenceExtensions
{
    public static IResourceBuilder<ProjectResource> AddPersistence(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> api)
    {
        _ = builder;
        return api;
    }
}