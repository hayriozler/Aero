using IataText.Parser.ValueObjects;

namespace IataText.Parser.Entities;

public sealed class TextMessage
{
    private TextMessage()  {   } //for eF core
    public long Id { get; set; }
    public MessageHeader Header { get; private set; } = default!;
    public MessageFooter Footer { get; private set; } = default!;
    public string Message { get; private set; } = string.Empty;    
    public required DateTime RecordDateTime
    {
        get; set;
    }
    public DateTime? ProcessDateTime
    {
        get; set;
    }
    public bool Completed
    {
        get; set;
    } = false;
    public string Status
    {
        get; set;
    } = "NOT_PROCESSED";

    /// <summary>UTC timestamp when Status was set to PROCESSING — used for stale-record recovery.</summary>
    public DateTime? ProcessingStartedAt { get; set; }
    public static TextMessage Create(string textMessage, MessageHeader header, MessageFooter footer) =>
    new()
    {
        Message = textMessage,
        Header = header,
        Footer = footer,
        RecordDateTime = DateTime.UtcNow
    };
}
