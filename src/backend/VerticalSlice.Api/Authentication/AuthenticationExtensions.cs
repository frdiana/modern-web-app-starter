namespace VerticalSlice.Api.Authentication;

internal static class AuthenticationExtensions
{
    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;
        services.AddAuthentication();
        services.AddAuthorization();
        return services;
    }

    public static RouteHandlerBuilder RequireConfiguredAuthorization(
        this RouteHandlerBuilder builder)
    {
        return builder;
    }
}