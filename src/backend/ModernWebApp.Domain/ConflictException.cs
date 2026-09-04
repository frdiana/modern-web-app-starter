namespace ModernWebApp.Domain;

public sealed class ConflictException(string message) : Exception(message);