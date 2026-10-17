using Aero.Flights.Domain.Common;
using Aero.Shared.Domain;

namespace Aero.Flights.Domain.Ops.MovementMessages;

public abstract class MovementMessage : FlightResourceBase
{
    public string MessageType { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public DateTimeOffset ReceivedAt { get; private set; }

    protected MovementMessage() { }

    protected MovementMessage(long flightId, string messageType, string content, DateTimeOffset receivedAt)
        : base(flightId)
    {
        MessageType = messageType;
        Content = content;
        ReceivedAt = receivedAt;
    }
}
