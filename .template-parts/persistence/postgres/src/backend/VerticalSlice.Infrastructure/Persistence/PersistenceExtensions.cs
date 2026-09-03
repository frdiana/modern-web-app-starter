using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VerticalSlice.Domain;

namespace VerticalSlice.Infrastructure.Persistence;

internal static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DatabaseConnectionOptions>().ValidateOnStart();
        services.AddSingleton<IValidateOptions<DatabaseConnectionOptions>, DatabaseConnectionOptionsValidator>();
        services.AddSingleton<PostgresConnectionFactory>();
        services.AddSingleton<IGreetingRepository, PostgresGreetingRepository>();
        return services;
    }
}