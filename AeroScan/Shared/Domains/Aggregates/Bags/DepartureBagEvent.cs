using Domain.Common;
using Domain.Interfaces;

namespace Domain.Aggregates.Bags;

public sealed class DepartureBagEvent : Entity<long>, IBagEvent
{
    public  DepartureBag DepartureBag { get; private set; } = null!;

    public long BagId  { get; private set; }

    public long? MessageId { get; private set; }

    public int? DeviceId { get; private set; }

    public string? UserName { get; private set; }

    public string EventId { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public DateTime EventTime { get; private set; }

    private DepartureBagEvent(){}
    public static DepartureBagEvent Create(
        DepartureBag departureBag,
        long? messageId,
        string eventId,
        int? deviceId = null,
        string? userName = null) => new()
        {
            DepartureBag = departureBag,
            MessageId = messageId,
            EventId = eventId,
            DeviceId = deviceId,
            UserName = userName,
            EventTime = DateTime.UtcNow
        };
}
