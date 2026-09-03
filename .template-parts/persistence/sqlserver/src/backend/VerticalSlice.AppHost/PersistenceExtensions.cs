using Aspire.Hosting.ApplicationModel;

internal static class PersistenceExtensions
{
    public static IResourceBuilder<ProjectResource> AddPersistence(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> api)
    {
        var database = builder.AddSqlServer("sql")
            .WithDataVolume()
            .AddDatabase("greetings");

        api.WithReference(database).WaitFor(database);
        return api;
    }
}