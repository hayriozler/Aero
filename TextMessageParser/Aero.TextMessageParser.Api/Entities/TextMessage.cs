using Aero.TextMessageParser.Api.ValueObjects;

namespace Aero.TextMessageParser.Api.Entities;

public enum TextMessageStatus
{
    NotProcessed,
    Processing,
    Processed,
    Error
}

public static class TextMessageStatusStore
{
    public const string NotProcessed = "NOT_PROCESSED";
    public const string Processing = "PROCESSING";
    public const string Processed = "PROCESSED";
    public const string Error = "ERROR";

    public static string ToStored(TextMessageStatus status) => status switch
    {
        TextMessageStatus.NotProcessed => NotProcessed,
        TextMessageStatus.Processing => Processing,
        TextMessageStatus.Processed => Processed,
        TextMessageStatus.Error => Error,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    public static TextMessageStatus FromStored(string stored) => stored switch
    {
        NotProcessed => TextMessageStatus.NotProcessed,
        Processing => TextMessageStatus.Processing,
        Processed => TextMessageStatus.Processed,
        Error => TextMessageStatus.Error,
        _ => throw new ArgumentOutOfRangeException(nameof(stored), stored, null)
    };
}

public sealed class TextMessage
{
    private TextMessage() { }
    public long Id { get; internal set; }
    public MessageHeader Header { get; private set; } = default!;
    public MessageFooter Footer { get; private set; } = default!;
    public string Message { get; private set; } = string.Empty;
    public DateTime ReceivedAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public bool Completed => Status is TextMessageStatus.Processed or TextMessageStatus.Error;
    public TextMessageStatus Status { get; private set; } = TextMessageStatus.NotProcessed;
    public DateTime? ProcessingStartedAt { get; private set; }
    public int AttemptCount { get; private set; }

    public static TextMessage Create(string textMessage, MessageHeader header, MessageFooter footer) =>
    new()
    {
        Message = textMessage,
        Header = header,
        Footer = footer,
        ReceivedAtUtc = DateTime.UtcNow
    };

    public void MarkProcessed(DateTime processedAtUtc) => Complete(TextMessageStatus.Processed, processedAtUtc);

    public void MarkFailed(DateTime processedAtUtc) => Complete(TextMessageStatus.Error, processedAtUtc);

    public bool TryRelease(int maxAttempts)
    {
        if (Status != TextMessageStatus.Processing)
            throw new InvalidOperationException(
                $"Text message {Id} is {Status}; only a claimed ({TextMessageStatus.Processing}) message can be released.");
        if (AttemptCount >= maxAttempts)
            return false;
        Status = TextMessageStatus.NotProcessed;
        ProcessingStartedAt = null;
        return true;
    }

    private void Complete(TextMessageStatus status, DateTime processedAtUtc)
    {
        if (Status != TextMessageStatus.Processing)
            throw new InvalidOperationException(
                $"Text message {Id} is {Status}; only a claimed ({TextMessageStatus.Processing}) message can be completed.");
        Status = status;
        ProcessedAtUtc = processedAtUtc;
    }
}
