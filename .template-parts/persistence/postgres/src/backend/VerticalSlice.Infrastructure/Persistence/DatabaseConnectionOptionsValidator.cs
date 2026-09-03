using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace VerticalSlice.Infrastructure.Persistence;

internal sealed class DatabaseConnectionOptionsValidator(IConfiguration configuration)
    : IValidateOptions<DatabaseConnectionOptions>
{
    public ValidateOptionsResult Validate(string? name, DatabaseConnectionOptions options)
    {
        _ = name;
        _ = options;

        return string.IsNullOrWhiteSpace(configuration.GetConnectionString("greetings"))
            ? ValidateOptionsResult.Fail("ConnectionStrings:greetings is required.")
            : ValidateOptionsResult.Success;
    }
}