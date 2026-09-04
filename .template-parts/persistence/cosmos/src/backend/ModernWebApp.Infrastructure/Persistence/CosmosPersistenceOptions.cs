namespace ModernWebApp.Infrastructure.Persistence;

internal sealed class CosmosPersistenceOptions
{
    public const string SectionName = "Cosmos";

    public string DatabaseName { get; init; } = "greetings";

    public string ContainerName { get; init; } = "greetings";
}