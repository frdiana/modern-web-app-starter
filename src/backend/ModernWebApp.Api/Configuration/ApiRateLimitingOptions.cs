namespace ModernWebApp.Api.Configuration;

public sealed class ApiRateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public int PermitLimit { get; init; }

    public int WindowSeconds { get; init; }

    public int QueueLimit { get; init; }
}