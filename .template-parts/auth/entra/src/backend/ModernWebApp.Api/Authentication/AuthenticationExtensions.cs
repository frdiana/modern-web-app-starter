using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;

namespace ModernWebApp.Api.Authentication;

internal static class AuthenticationExtensions
{
    private const string ApiScopePolicy = "ApiScope";

    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection(EntraOptions.SectionName);

        services.AddOptions<EntraOptions>()
            .Bind(section)
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<EntraOptions>, EntraOptionsValidator>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(section);

        var scope = section[nameof(EntraOptions.Scope)]
            ?? throw new InvalidOperationException("Entra:Scope is required.");

        services.AddAuthorizationBuilder()
            .AddPolicy(ApiScopePolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context => context.User
                    .FindAll("scp")
                    .SelectMany(static claim => claim.Value.Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries))
                    .Contains(scope, StringComparer.Ordinal));
            });
        services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecurityTransformer>());

        return services;
    }

    public static RouteHandlerBuilder RequireConfiguredAuthorization(
        this RouteHandlerBuilder builder)
    {
        return builder.RequireAuthorization(ApiScopePolicy);
    }
}

internal sealed class BearerSecurityTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        _ = context;
        cancellationToken.ThrowIfCancellationRequested();

        const string schemeName = "Bearer";
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>(StringComparer.Ordinal);
        document.Components.SecuritySchemes[schemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Microsoft Entra ID access token"
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(schemeName, document, null)] =
                []
        });

        return Task.CompletedTask;
    }
}