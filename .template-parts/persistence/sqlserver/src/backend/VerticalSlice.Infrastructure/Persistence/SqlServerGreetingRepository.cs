using Dapper;
using VerticalSlice.Domain;

namespace VerticalSlice.Infrastructure.Persistence;

internal sealed class SqlServerGreetingRepository(SqlConnectionFactory connectionFactory)
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
            INSERT INTO dbo.Greetings (Id, Message, CreatedAt)
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
            SELECT TOP (100) Id, Message, CreatedAt
            FROM dbo.Greetings
            ORDER BY CreatedAt DESC;
            """;
        var result = await connection.QueryAsync<Greeting>(new CommandDefinition(
            sql,
            cancellationToken: cancellationToken));

        return result.AsList();
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
                BEGIN TRANSACTION;
                DECLARE @lockResult int;
                EXEC @lockResult = sp_getapplock
                    @Resource = N'VerticalSlice.Greetings.Schema',
                    @LockMode = N'Exclusive',
                    @LockOwner = N'Transaction',
                    @LockTimeout = 15000;
                IF @lockResult < 0
                    THROW 51000, 'Could not acquire the greetings schema lock.', 1;

                IF OBJECT_ID(N'dbo.Greetings', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Greetings
                    (
                        Id uniqueidentifier NOT NULL PRIMARY KEY,
                        Message nvarchar(200) NOT NULL,
                        CreatedAt datetimeoffset NOT NULL
                    );
                END;
                COMMIT TRANSACTION;
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
}