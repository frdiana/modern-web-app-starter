using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using ModernWebApp.Api.Endpoints.Examples;

namespace ModernWebApp.IntegrationTests;

public sealed class EchoEndpointTests(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>
{
    [Fact]
    public async Task Get_WithValidMessage_ReturnsResponse()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/examples/echo?message=hello", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadFromJsonAsync<Echo.Response>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal("hello", content.Message);
    }

    [Fact]
    public async Task Get_WithoutMessage_ReturnsValidationProblem()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/examples/echo", TestContext.Current.CancellationToken);
        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(problem);
        Assert.Contains("Message", problem.Errors.Keys);
    }
}