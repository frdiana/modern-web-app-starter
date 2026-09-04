using Microsoft.Extensions.Options;

namespace ModernWebApp.Api.Configuration;

internal sealed class ApiRateLimitingOptionsValidator : IValidateOptions<ApiRateLimitingOptions>
{
    public ValidateOptionsResult Validate(string? name, ApiRateLimitingOptions options)
    {
        _ = name;
        var failures = new List<string>();

        if (options.PermitLimit <= 0)
        {
            failures.Add("RateLimiting:PermitLimit must be greater than zero.");
        }

        if (options.WindowSeconds <= 0)
        {
            failures.Add("RateLimiting:WindowSeconds must be greater than zero.");
        }

        if (options.QueueLimit < 0)
        {
            failures.Add("RateLimiting:QueueLimit cannot be negative.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}