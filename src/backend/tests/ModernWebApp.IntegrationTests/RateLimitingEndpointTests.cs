using System.Net;

namespace ModernWebApp.IntegrationTests;

public sealed class RateLimitingEndpointTests
{
    [Fact]
    public async Task Echo_AfterPermitLimit_ReturnsTooManyRequests()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        HttpResponseMessage? rejectedResponse = null;

        for (var requestNumber = 0; requestNumber < 61; requestNumber++)
        {
            var response = await client.GetAsync(
                "/api/examples/echo?message=hello",
                TestContext.Current.CancellationToken);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                rejectedResponse = response;
                break;
            }

            response.Dispose();
        }

        Assert.NotNull(rejectedResponse);
        rejectedResponse.Dispose();
    }

}