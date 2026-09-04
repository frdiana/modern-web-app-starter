namespace ModernWebApp.Api.Extensions;

internal static class AuthenticationExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApiAuthentication(IConfiguration configuration)
        {
            _ = configuration;
            services.AddAuthentication();
            services.AddAuthorization();
            return services;
        }
    }

    extension(RouteHandlerBuilder builder)
    {
        public RouteHandlerBuilder RequireConfiguredAuthorization()
        {
            return builder;
        }
    }
}