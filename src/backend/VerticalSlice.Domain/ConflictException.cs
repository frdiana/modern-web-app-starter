namespace VerticalSlice.Domain;

public sealed class ConflictException(string message) : Exception(message);