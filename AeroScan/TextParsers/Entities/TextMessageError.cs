namespace IataText.Parser.Entities;

public sealed class TextMessageError 
{
    public int Id { get; private set; }
    public long MessageId { get; private set; }
    public string ErrorCode { get; private set; } = null!;
    public string? ErrorDescription { get; private set; } = null!;
    public DateTime RecordDateTime { get; private set; }
    private TextMessageError(){    }
    public static TextMessageError Create(long messageId, string errorCode, string? errorDescription) =>
    new()
    {
        MessageId = messageId,
        ErrorCode = errorCode,
        ErrorDescription = errorDescription,
        RecordDateTime = DateTime.UtcNow
    };
}
