using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ModernWebApp.Infrastructure.Persistence;

internal sealed class CosmosPersistenceOptionsValidator(IConfiguration configuration)
    : IValidateOptions<CosmosPersistenceOptions>
{
    public ValidateOptionsResult Validate(string? name, CosmosPersistenceOptions options)
    {
        _ = name;
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("greetings")))
        {
            failures.Add("ConnectionStrings:greetings is required.");
        }

        if (string.IsNullOrWhiteSpace(options.DatabaseName))
        {
            failures.Add("Cosmos:DatabaseName is required.");
        }

        if (string.IsNullOrWhiteSpace(options.ContainerName))
        {
            failures.Add("Cosmos:ContainerName is required.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}