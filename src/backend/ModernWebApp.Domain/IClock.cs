namespace ModernWebApp.Domain;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}