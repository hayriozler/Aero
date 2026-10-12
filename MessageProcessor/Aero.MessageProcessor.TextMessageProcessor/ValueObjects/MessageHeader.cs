namespace Aero.MessageProcessor.TextMessageProcessor.ValueObjects;

public sealed record MessageHeader(string Identifier, string? SecondaryIdentifier, string? ChangeOfStatus);
