namespace ModernWebApp.Domain;

public sealed record Greeting(Guid Id, string Message, DateTimeOffset CreatedAt);