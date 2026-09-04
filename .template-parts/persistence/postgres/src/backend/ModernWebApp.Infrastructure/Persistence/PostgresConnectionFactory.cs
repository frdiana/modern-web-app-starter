using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ModernWebApp.Infrastructure.Persistence;

internal sealed class PostgresConnectionFactory(IConfiguration configuration)
{
    public async Task<DbConnection> OpenAsync(CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(
            configuration.GetConnectionString("greetings")
                ?? throw new InvalidOperationException("ConnectionStrings:greetings is required."));
        try
        {
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }
}