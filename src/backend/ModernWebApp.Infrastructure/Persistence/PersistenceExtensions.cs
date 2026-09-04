using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModernWebApp.Domain;

namespace ModernWebApp.Infrastructure.Persistence;

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