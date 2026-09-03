using Microsoft.Azure.Cosmos;
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
        services.AddOptions<CosmosPersistenceOptions>()
            .Bind(configuration.GetSection(CosmosPersistenceOptions.SectionName))
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<CosmosPersistenceOptions>, CosmosPersistenceOptionsValidator>();

        services.AddSingleton(_ => new CosmosClient(
            configuration.GetConnectionString("greetings")
                ?? throw new InvalidOperationException("ConnectionStrings:greetings is required."),
            new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true,
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            }));
        services.AddSingleton<IGreetingRepository, CosmosGreetingRepository>();

        return services;
    }
}