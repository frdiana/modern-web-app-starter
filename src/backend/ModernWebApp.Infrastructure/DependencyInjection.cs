using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModernWebApp.Domain;
using ModernWebApp.Infrastructure.Persistence;

namespace ModernWebApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;
        services.AddSingleton<IClock, SystemClock>();
        services.AddPersistence(configuration);

        return services;
    }
}