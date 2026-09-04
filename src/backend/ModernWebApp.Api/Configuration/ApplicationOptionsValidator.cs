using Microsoft.Extensions.Options;

namespace ModernWebApp.Api.Configuration;

internal sealed class ApplicationOptionsValidator : IValidateOptions<ApplicationOptions>
{
    public ValidateOptionsResult Validate(string? name, ApplicationOptions options)
    {
        _ = name;

        return string.IsNullOrWhiteSpace(options.Name)
            ? ValidateOptionsResult.Fail("Application:Name is required.")
            : ValidateOptionsResult.Success;
    }
}