using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using VerticalSlice.Domain;

namespace VerticalSlice.Infrastructure.Persistence;

internal sealed class CosmosGreetingRepository(
    CosmosClient client,
    IOptions<CosmosPersistenceOptions> options) : IGreetingRepository
{
    private const string PartitionKeyValue = "greetings";
    private readonly SemaphoreSlim initializationLock = new(1, 1);
    private Container? container;

    public async Task<Greeting> AddAsync(
        Greeting greeting,
        CancellationToken cancellationToken)
    {
        var target = await GetContainerAsync(cancellationToken);
        var document = new GreetingDocument(
            greeting.Id,
            PartitionKeyValue,
            greeting.Message,
            greeting.CreatedAt);

        await target.UpsertItemAsync(
            document,
            new PartitionKey(PartitionKeyValue),
            cancellationToken: cancellationToken);

        return greeting;
    }

    public async Task<IReadOnlyList<Greeting>> ListAsync(CancellationToken cancellationToken)
    {
        var target = await GetContainerAsync(cancellationToken);
        var query = target.GetItemQueryIterator<GreetingDocument>(
            new QueryDefinition(
                "SELECT TOP 100 * FROM item WHERE item.partitionKey = @partitionKey ORDER BY item.createdAt DESC")
                .WithParameter("@partitionKey", PartitionKeyValue));
        var result = new List<Greeting>();

        while (query.HasMoreResults)
        {
            var page = await query.ReadNextAsync(cancellationToken);
            result.AddRange(page.Select(static item => new Greeting(
                item.Id,
                item.Message,
                item.CreatedAt)));
        }

        return result;
    }

    private async Task<Container> GetContainerAsync(CancellationToken cancellationToken)
    {
        if (container is not null)
        {
            return container;
        }

        await initializationLock.WaitAsync(cancellationToken);
        try
        {
            if (container is not null)
            {
                return container;
            }

            var settings = options.Value;
            var database = await client.CreateDatabaseIfNotExistsAsync(
                settings.DatabaseName,
                cancellationToken: cancellationToken);
            var created = await database.Database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(settings.ContainerName, "/partitionKey"),
                cancellationToken: cancellationToken);
            container = created.Container;

            return container;
        }
        finally
        {
            initializationLock.Release();
        }
    }

    private sealed record GreetingDocument(
        Guid Id,
        string PartitionKey,
        string Message,
        DateTimeOffset CreatedAt);
}