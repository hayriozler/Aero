namespace Domain.Interfaces;

public interface IBagEvent
{
    public long BagId { get; }
    public long? MessageId { get; }
    public int? DeviceId { get; }
    public string? UserName { get; }
    public string EventId { get; }
    public string? Description { get; }
    public DateTime EventTime { get; }
}
