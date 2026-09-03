namespace VerticalSlice.Domain;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}