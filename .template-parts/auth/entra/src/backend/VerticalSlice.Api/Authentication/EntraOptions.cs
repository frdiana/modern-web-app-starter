namespace VerticalSlice.Api.Authentication;

internal sealed class EntraOptions
{
    public const string SectionName = "Entra";

    public string Instance { get; init; } = string.Empty;

    public string TenantId { get; init; } = string.Empty;

    public string ClientId { get; init; } = string.Empty;

    public string Scope { get; init; } = string.Empty;
}