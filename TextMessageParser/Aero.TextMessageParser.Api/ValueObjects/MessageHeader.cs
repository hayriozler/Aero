namespace Aero.TextMessageParser.Api.ValueObjects;

public sealed record MessageHeader(string Identifier, string? SecondaryIdentifier, string? ChangeOfStatus);
