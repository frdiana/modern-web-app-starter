using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModernWebApp.Api.Endpoints.Examples;
using ModernWebApp.Domain;

namespace ModernWebApp.IntegrationTests;

public sealed class EchoEndpointTests : IClassFixture<EntraWebApplicationFactory>
{
    private readonly EntraWebApplicationFactory factory;

    public EchoEndpointTests(EntraWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Get_WithoutAccessToken_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(
            "/api/examples/echo?message=hello",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithoutAccessToken_AfterPermitLimit_ReturnsTooManyRequests()
    {
        await using var isolatedFactory = new EntraWebApplicationFactory();
        using var client = isolatedFactory.CreateClient();
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

    [Fact]
    public async Task Get_WithRequiredScope_ReturnsResponse()
    {
        await using var authenticatedFactory = new AuthenticatedEntraWebApplicationFactory();
        using var client = authenticatedFactory.CreateClient();

        using var response = await client.GetAsync(
            "/api/examples/echo?message=hello",
            TestContext.Current.CancellationToken);
        var content = await response.Content.ReadFromJsonAsync<Echo.Response>(
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("hello", content?.Message);
    }

    [Fact]
    public async Task Get_WithWrongScope_ReturnsForbidden()
    {
        await using var authenticatedFactory = new AuthenticatedEntraWebApplicationFactory();
        using var client = authenticatedFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Scope", "different_scope");

        using var response = await client.GetAsync(
            "/api/examples/echo?message=hello",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateThenList_WithRequiredScope_ReturnsStoredGreeting()
    {
        await using var authenticatedFactory = new AuthenticatedEntraWebApplicationFactory();
        using var client = authenticatedFactory.CreateClient();

        using var createResponse = await client.PostAsJsonAsync(
            "/api/examples/greetings",
            new CreateGreeting.Request("Protected greeting"),
            TestContext.Current.CancellationToken);
        using var listResponse = await client.GetAsync(
            "/api/examples/greetings",
            TestContext.Current.CancellationToken);
        var greetings = await listResponse.Content.ReadFromJsonAsync<ListGreetings.Response[]>(
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Contains(greetings!, static greeting => greeting.Message == "Protected greeting");
    }

    [Fact]
    public async Task Echo_WhenFeatureDisabled_ReturnsNotFound()
    {
        await using var authenticatedFactory = new DisabledFeatureEntraWebApplicationFactory();
        using var client = authenticatedFactory.CreateClient();

        using var response = await client.GetAsync(
            "/api/examples/echo?message=hello",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Echo_AfterUserPermitLimit_ReturnsTooManyRequests()
    {
        await using var authenticatedFactory = new AuthenticatedEntraWebApplicationFactory();
        using var client = authenticatedFactory.CreateClient();
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

    [Fact]
    public async Task OpenApi_ContainsBearerSecurityScheme()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(
            "/openapi/v1.json",
            TestContext.Current.CancellationToken);
        using var document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken),
            cancellationToken: TestContext.Current.CancellationToken);

        var securitySchemes = document.RootElement
            .GetProperty("components")
            .GetProperty("securitySchemes");

        Assert.True(response.IsSuccessStatusCode);
        Assert.True(securitySchemes.TryGetProperty("Bearer", out _));
    }
}

public sealed class EntraWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Entra:Instance"] = "https://login.microsoftonline.com/",
                ["Entra:TenantId"] = "00000000-0000-0000-0000-000000000001",
                ["Entra:ClientId"] = "00000000-0000-0000-0000-000000000002",
                ["Entra:Scope"] = "access_as_user",
                ["ConnectionStrings:greetings"] = "AccountEndpoint=https://localhost:8081;AccountKey=test;"
            });
        });
    }
}

public class AuthenticatedEntraWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Entra:Instance"] = "https://login.microsoftonline.com/",
                ["Entra:TenantId"] = "00000000-0000-0000-0000-000000000001",
                ["Entra:ClientId"] = "00000000-0000-0000-0000-000000000002",
                ["Entra:Scope"] = "access_as_user",
                ["ConnectionStrings:greetings"] = "AccountEndpoint=https://localhost:8081;AccountKey=test;"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IGreetingRepository>();
            services.AddSingleton<IGreetingRepository, TestGreetingRepository>();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.SchemeName,
                    _ => { });
        });
    }
}

public sealed class DisabledFeatureEntraWebApplicationFactory
    : AuthenticatedEntraWebApplicationFactory
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

internal sealed class TestGreetingRepository : IGreetingRepository
{
    private readonly List<Greeting> greetings = [];

    public Task<Greeting> AddAsync(Greeting greeting, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        greetings.Add(greeting);
        return Task.FromResult(greeting);
    }

    public Task<IReadOnlyList<Greeting>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<Greeting> result = greetings
            .OrderByDescending(static greeting => greeting.CreatedAt)
            .Take(100)
            .ToArray();
        return Task.FromResult(result);
    }
}

internal sealed class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var scope = Request.Headers.TryGetValue("X-Test-Scope", out var requestedScope)
            ? requestedScope.ToString()
            : "access_as_user";
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, "test-user"),
                new Claim("scp", scope)
            ],
            SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}