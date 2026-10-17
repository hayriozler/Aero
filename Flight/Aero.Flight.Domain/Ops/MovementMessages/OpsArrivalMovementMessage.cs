namespace Aero.Flights.Domain.Ops.MovementMessages;

public sealed class OpsArrivalMovementMessage : MovementMessage
{
    private OpsArrivalMovementMessage() { }

    private OpsArrivalMovementMessage(long flightId, string messageType, string content, DateTimeOffset receivedAt)
        : base(flightId, messageType, content, receivedAt) { }

    public static OpsArrivalMovementMessage Create(long flightId, string messageType, string content, DateTimeOffset receivedAt)
        => new(flightId, messageType, content, receivedAt);
}
