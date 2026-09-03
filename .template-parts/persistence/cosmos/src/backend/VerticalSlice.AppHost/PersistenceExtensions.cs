using Aspire.Hosting.ApplicationModel;

internal static class PersistenceExtensions
{
    public static IResourceBuilder<ProjectResource> AddPersistence(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> api)
    {
#pragma warning disable ASPIRECOSMOSDB001
        var database = builder.AddAzureCosmosDB("cosmos")
            .RunAsPreviewEmulator(static emulator => emulator.WithDataVolume())
            .AddCosmosDatabase("greetings");
#pragma warning restore ASPIRECOSMOSDB001

        api.WithReference(database).WaitFor(database);
        return api;
    }
}