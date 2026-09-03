namespace VerticalSlice.Domain;

public sealed record Greeting(Guid Id, string Message, DateTimeOffset CreatedAt);