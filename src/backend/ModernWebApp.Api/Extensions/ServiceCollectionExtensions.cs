using System.Threading.RateLimiting;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using ModernWebApp.Api.Configuration;
using ModernWebApp.Api.Features;

namespace ModernWebApp.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ApplicationOptions>()
            .Bind(configuration.GetRequiredSection(ApplicationOptions.SectionName))
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<ApplicationOptions>, ApplicationOptionsValidator>();

        services.AddOptions<ApiRateLimitingOptions>()
            .Bind(configuration.GetRequiredSection(ApiRateLimitingOptions.SectionName))
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<ApiRateLimitingOptions>, ApiRateLimitingOptionsValidator>();

        return services;
    }

    public static IServiceCollection AddApiFeatureManagement(this IServiceCollection services)
    {
        services.AddFeatureManagement();
        return services;
    }

    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration
            .GetRequiredSection(ApiRateLimitingOptions.SectionName)
            .Get<ApiRateLimitingOptions>()
            ?? throw new InvalidOperationException("Rate limiting configuration is required.");

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(RateLimitingPolicies.Api, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetPartitionKey(context),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                        QueueLimit = settings.QueueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }));
        });
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownProxies.Clear();

            var knownProxies = configuration
                .GetRequiredSection("ReverseProxy:KnownProxies")
                .Get<string[]>()
                ?? throw new InvalidOperationException("ReverseProxy:KnownProxies is required.");

            foreach (var value in knownProxies)
            {
                if (!IPAddress.TryParse(value, out var address))
                {
                    throw new InvalidOperationException(
                        $"ReverseProxy:KnownProxies contains invalid IP address '{value}'.");
                }

                options.KnownProxies.Add(address);
            }
        });

        return services;
    }

    internal static string GetPartitionKey(HttpContext context)
    {
        var subject = context.User.FindFirst("sub")?.Value
            ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return subject is null
            ? $"ip:{context.Connection.RemoteIpAddress}"
            : $"user:{subject}";
    }
}