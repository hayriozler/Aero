namespace Aero.Flights.Domain.Messaging;

public sealed class InboxMessage
{
    public long Id { get; private set; }
    public string Source { get; private set; } = string.Empty;
    public string MessageType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTimeOffset ReceivedAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public int Attempts { get; private set; }
    public string? LastError { get; private set; }

    private InboxMessage() { }

    private InboxMessage(string source, string messageType, string payload, DateTimeOffset receivedAt)
    {
        Source = source;
        MessageType = messageType;
        Payload = payload;
        ReceivedAt = receivedAt;
    }

    public static InboxMessage Create(string source, string messageType, string payload, DateTimeOffset receivedAt)
        => new(source, messageType, payload, receivedAt);

    public void MarkProcessed(DateTimeOffset at) => ProcessedAt = at;

    // ProcessedAt is set here too — a failed message is still "processed" in the sense that it
    // won't be picked up again by the unprocessed-scan (ProcessedAt IS NULL); otherwise a
    // permanently-unmatchable message (bad flight number, stale data) gets retried forever.
    public void MarkFailed(string error, DateTimeOffset at)
    {
        Attempts++;
        LastError = error;
        ProcessedAt = at;
    }
}
