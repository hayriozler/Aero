using Aero.Flights.Application.Flights.MovementMessages;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Inbound;
using Aero.Flights.Contracts.Topology;
using Aero.Flights.Domain.Ops.MovementMessages;

namespace Aero.Flights.Application.Flights.InboundProcessors;

public sealed class MovementMessageReceivedProcessor(
    IAeroFlightDbContext db,
    IMovementMessageFactory<OpsArrivalMovementMessage> arrivalFactory,
    IMovementMessageFactory<OpsDepartureMovementMessage> departureFactory)
    : FlightInboundProcessor<MovementMessageReceived>(db)
{
    public override string MessageType => RoutingKeys.Inbound.MovementMessageReceived;

    protected override async Task ApplyAsync(MovementMessageReceived m, CancellationToken ct)
    {
        if (m.IsArrival)
        {
            var flight = await FindArrivalAsync(m.Id, m.FlightNumber, m.Scheduled, ct);
            Db.Add(arrivalFactory.Create(flight.Id, m.MessageType, m.Content, m.ReceivedAt));
        }
        else
        {
            var flight = await FindDepartureAsync(m.Id, m.FlightNumber, m.Scheduled, ct);
            Db.Add(departureFactory.Create(flight.Id, m.MessageType, m.Content, m.ReceivedAt));
        }

        await Db.SaveChangesAsync(ct);
    }
}
