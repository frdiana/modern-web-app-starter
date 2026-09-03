using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VerticalSlice.Domain;

namespace VerticalSlice.Infrastructure.Persistence;

internal static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;
        services.AddSingleton<IGreetingRepository, InMemoryGreetingRepository>();
        return services;
    }
}