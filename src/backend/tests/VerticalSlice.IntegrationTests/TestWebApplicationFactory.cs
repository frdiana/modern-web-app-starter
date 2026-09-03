using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VerticalSlice.Domain;

namespace VerticalSlice.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:greetings"] = "AccountEndpoint=https://localhost:8081;AccountKey=test;"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IGreetingRepository>();
            services.AddSingleton<IGreetingRepository, TestGreetingRepository>();
        });
    }
}

internal sealed class TestGreetingRepository : IGreetingRepository
{
    private readonly List<Greeting> greetings = [];
    private readonly Lock syncRoot = new();

    public Task<Greeting> AddAsync(Greeting greeting, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (syncRoot)
        {
            greetings.Add(greeting);
        }

        return Task.FromResult(greeting);
    }

    public Task<IReadOnlyList<Greeting>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (syncRoot)
        {
            IReadOnlyList<Greeting> result = greetings
                .OrderByDescending(static greeting => greeting.CreatedAt)
                .Take(100)
                .ToArray();
            return Task.FromResult(result);
        }
    }
}