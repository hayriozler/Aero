using Domain.Common;
using Domain.Interfaces;

namespace Domain.Aggregates.Bags;

public sealed class ArrivalBagEvent : Entity<long>, IBagEvent
{
    public ArrivalBag ArrivalBag { get; private set; } = null!;

    public long BagId { get; private set; }

    public long? MessageId { get; private set; }

    public int? DeviceId { get; private set; }

    public string? UserName { get; private set; }

    public string EventId { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public DateTime EventTime { get; private set; }

    private ArrivalBagEvent() { }
    internal static ArrivalBagEvent Create(
        ArrivalBag arrivalBag,
        long? messageId,
        string eventId,
        string description,
        int? deviceId,
        string? userName) => new()
        {
            ArrivalBag = arrivalBag,
            MessageId = messageId,
            EventId = eventId,
            Description = description,
            DeviceId = deviceId,
            UserName = userName,
            EventTime = DateTime.UtcNow
        };
}
