using System.Net;
using System.Net.Http.Json;
using ModernWebApp.Api.Endpoints.Examples;

namespace ModernWebApp.IntegrationTests;

public sealed class GreetingEndpointsTests(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>
{
    [Fact]
    public async Task CreateThenList_ReturnsStoredGreeting()
    {
        using var client = factory.CreateClient();
        var request = new CreateGreeting.Request("Hello repository");

        using var createResponse = await client.PostAsJsonAsync(
            "/api/examples/greetings",
            request,
            TestContext.Current.CancellationToken);
        using var listResponse = await client.GetAsync(
            "/api/examples/greetings",
            TestContext.Current.CancellationToken);
        var greetings = await listResponse.Content.ReadFromJsonAsync<ListGreetings.Response[]>(
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.Contains(greetings!, static greeting => greeting.Message == "Hello repository");
    }
}