namespace Aero.TextMessageParser.Api.Entities;

public sealed class ElementError
{
    public long Id { get; private set; }
    public long MessageId { get; private set; }
    public string ErrorCode { get; private set; } = null!;
    public string? ErrorDescription { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    private ElementError() { }
    public static ElementError Create(long messageId, string errorCode, string? errorDescription) =>
    new()
    {
        MessageId = messageId,
        ErrorCode = errorCode,
        ErrorDescription = errorDescription,
        CreatedAtUtc = DateTime.UtcNow
    };
}
