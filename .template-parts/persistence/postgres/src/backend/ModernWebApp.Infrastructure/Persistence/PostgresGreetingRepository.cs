using Dapper;
using ModernWebApp.Domain;

namespace ModernWebApp.Infrastructure.Persistence;

internal sealed class PostgresGreetingRepository(PostgresConnectionFactory connectionFactory)
    : IGreetingRepository
{
    private readonly SemaphoreSlim initializationLock = new(1, 1);
    private bool initialized;

    public async Task<Greeting> AddAsync(
        Greeting greeting,
        CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        const string sql = """
            INSERT INTO greetings (id, message, created_at)
            VALUES (@Id, @Message, @CreatedAt);
            """;
        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            greeting,
            cancellationToken: cancellationToken));

        return greeting;
    }

    public async Task<IReadOnlyList<Greeting>> ListAsync(CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        const string sql = """
            SELECT id AS Id, message AS Message, created_at AS CreatedAt
            FROM greetings
            ORDER BY created_at DESC
            LIMIT 100;
            """;
        var rows = await connection.QueryAsync<GreetingRow>(new CommandDefinition(
            sql,
            cancellationToken: cancellationToken));

        return rows
            .Select(static row => new Greeting(
                row.Id,
                row.Message,
                new DateTimeOffset(DateTime.SpecifyKind(row.CreatedAt, DateTimeKind.Utc))))
            .ToArray();
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (initialized)
        {
            return;
        }

        await initializationLock.WaitAsync(cancellationToken);
        try
        {
            if (initialized)
            {
                return;
            }

            await using var connection = await connectionFactory.OpenAsync(cancellationToken);
            const string sql = """
                CREATE TABLE IF NOT EXISTS greetings
                (
                    id uuid PRIMARY KEY,
                    message varchar(200) NOT NULL,
                    created_at timestamp with time zone NOT NULL
                );
                """;
            await connection.ExecuteAsync(new CommandDefinition(
                sql,
                cancellationToken: cancellationToken));
            initialized = true;
        }
        finally
        {
            initializationLock.Release();
        }
    }

    private sealed class GreetingRow
    {
        public Guid Id { get; init; }

        public string Message { get; init; } = string.Empty;

        public DateTime CreatedAt { get; init; }
    }
}