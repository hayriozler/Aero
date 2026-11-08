using Aero.TextMessageParser.Api.ValueObjects;

namespace Aero.TextMessageParser.Api.Entities;

public static class TextMessageStatus
{
    public const string NotProcessed = "NOT_PROCESSED";
    public const string Processing = "PROCESSING";
    public const string Processed = "PROCESSED";
    public const string Error = "ERROR";
}

public sealed class TextMessage
{
    private TextMessage() { }
    public long Id { get; internal set; }
    public MessageHeader Header { get; private set; } = default!;
    public MessageFooter Footer { get; private set; } = default!;
    public string Message { get; private set; } = string.Empty;
    public DateTime RecordDateTime { get; private set; }
    public DateTime? ProcessDateTime { get; private set; }
    public bool Completed { get; private set; }
    public string Status { get; private set; } = TextMessageStatus.NotProcessed;
    public DateTime? ProcessingStartedAt { get; private set; }

    public static TextMessage Create(string textMessage, MessageHeader header, MessageFooter footer) =>
    new()
    {
        Message = textMessage,
        Header = header,
        Footer = footer,
        RecordDateTime = DateTime.UtcNow
    };

    public void MarkProcessed(DateTime processedAtUtc) => Complete(TextMessageStatus.Processed, processedAtUtc);

    public void MarkFailed(DateTime processedAtUtc) => Complete(TextMessageStatus.Error, processedAtUtc);

    private void Complete(string status, DateTime processedAtUtc)
    {
        if (Status != TextMessageStatus.Processing)
            throw new InvalidOperationException(
                $"Text message {Id} is {Status}; only a claimed ({TextMessageStatus.Processing}) message can be completed.");
        Status = status;
        Completed = true;
        ProcessDateTime = processedAtUtc;
    }
}
