namespace IataText.Parser.ValueObjects;

public sealed record MessageHeader(string Identifier, string? SecondaryIdentifier, string? ChangeOfStatus);
