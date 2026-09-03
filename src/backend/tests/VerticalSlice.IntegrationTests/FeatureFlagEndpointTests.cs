using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace VerticalSlice.IntegrationTests;

public sealed class FeatureFlagEndpointTests
{
    [Fact]
    public async Task Echo_WhenGreetingFeatureIsDisabled_ReturnsNotFound()
    {
        await using var factory = new DisabledGreetingFeatureFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(
            "/api/examples/echo?message=hello",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class DisabledGreetingFeatureFactory : TestWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["FeatureManagement:GreetingEndpoint"] = "false"
                });
            });
        }
    }
}