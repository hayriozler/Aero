namespace IataText.Parser.Entities;

public sealed class ElementError
{
    public int Id { get; private set; }
    public long MessageId { get; private set; }
    public string ErrorCode { get; private set; } = null!;
    public string? ErrorDescription { get; private set; } = null!;
    public DateTime RecordDateTime { get; private set; }
    private ElementError() { }
    public static ElementError Create(long messageId, string errorCode, string? errorDescription) =>
    new()
    {
        MessageId = messageId,
        ErrorCode = errorCode,
        ErrorDescription = errorDescription,
        RecordDateTime = DateTime.UtcNow
    };
}
