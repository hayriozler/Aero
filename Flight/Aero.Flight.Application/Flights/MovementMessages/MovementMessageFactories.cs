using Aero.Flights.Domain.Ops.MovementMessages;

namespace Aero.Flights.Application.Flights.MovementMessages;

public interface IMovementMessageFactory<TMessage> where TMessage : MovementMessage
{
    TMessage Create(long flightId, string messageType, string content, DateTimeOffset receivedAt);
}

public sealed class ArrivalMovementMessageFactory : IMovementMessageFactory<OpsArrivalMovementMessage>
{
    public OpsArrivalMovementMessage Create(long flightId, string messageType, string content, DateTimeOffset receivedAt) =>
        OpsArrivalMovementMessage.Create(flightId, messageType, content, receivedAt);
}

public sealed class DepartureMovementMessageFactory : IMovementMessageFactory<OpsDepartureMovementMessage>
{
    public OpsDepartureMovementMessage Create(long flightId, string messageType, string content, DateTimeOffset receivedAt) =>
        OpsDepartureMovementMessage.Create(flightId, messageType, content, receivedAt);
}
