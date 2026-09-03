using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace VerticalSlice.Infrastructure.Persistence;

internal sealed class SqlConnectionFactory(IConfiguration configuration)
{
    public async Task<DbConnection> OpenAsync(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(
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