namespace Aero.Flights.Domain.Ops.MovementMessages;

public sealed class OpsDepartureMovementMessage : MovementMessage
{
    private OpsDepartureMovementMessage() { }

    private OpsDepartureMovementMessage(long flightId, string messageType, string content, DateTimeOffset receivedAt)
        : base(flightId, messageType, content, receivedAt) { }

    public static OpsDepartureMovementMessage Create(long flightId, string messageType, string content, DateTimeOffset receivedAt)
        => new(flightId, messageType, content, receivedAt);
}
