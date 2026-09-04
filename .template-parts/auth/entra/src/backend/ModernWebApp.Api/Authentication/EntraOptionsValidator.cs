using Microsoft.Extensions.Options;

namespace ModernWebApp.Api.Authentication;

internal sealed class EntraOptionsValidator : IValidateOptions<EntraOptions>
{
    public ValidateOptionsResult Validate(string? name, EntraOptions options)
    {
        _ = name;
        var failures = new List<string>();

        if (!Uri.TryCreate(options.Instance, UriKind.Absolute, out _))
        {
            failures.Add("Entra:Instance must be an absolute URI.");
        }

        ValidateIdentifier(options.TenantId, "Entra:TenantId", failures);
        ValidateIdentifier(options.ClientId, "Entra:ClientId", failures);

        if (string.IsNullOrWhiteSpace(options.Scope)
            || options.Scope.Contains("YOUR_", StringComparison.OrdinalIgnoreCase))
        {
            failures.Add("Entra:Scope must contain the exposed API scope name.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static void ValidateIdentifier(
        string value,
        string key,
        ICollection<string> failures)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Contains("YOUR_", StringComparison.OrdinalIgnoreCase))
        {
            failures.Add($"{key} must be configured.");
        }
    }
}