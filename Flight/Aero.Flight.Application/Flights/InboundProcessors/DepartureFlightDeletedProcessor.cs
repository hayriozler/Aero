using Aero.Flights.Application.Common;
using Aero.Flights.Application.Flights.Commands;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Inbound;
using Aero.Flights.Contracts.Topology;

namespace Aero.Flights.Application.Flights.InboundProcessors;

// See ArrivalFlightDeletedProcessor — same resolve-then-delegate shape, for departures.
public sealed class DepartureFlightDeletedProcessor(IAeroFlightDbContext db, ICommandHandler<DeleteDepartureFlightCommand, bool> deleteHandler)
    : FlightInboundProcessor<FlightDeleted>(db)
{
    public override string MessageType => RoutingKeys.Inbound.DepartureFlightDeleted;

    protected override async Task ApplyAsync(FlightDeleted m, CancellationToken ct)
    {
        var f = await FindDepartureAsync(m.Id, m.FlightNumber, m.Scheduled, ct);
        await deleteHandler.HandleAsync(new DeleteDepartureFlightCommand(f.Id, m.DeleteLinkedFlight), ct);
    }
}
