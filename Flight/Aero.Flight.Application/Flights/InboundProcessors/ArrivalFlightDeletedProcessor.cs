using Aero.Flights.Application.Common;
using Aero.Flights.Application.Flights.Commands;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Inbound;
using Aero.Flights.Contracts.Topology;

namespace Aero.Flights.Application.Flights.InboundProcessors;

// Resolves the flight (id-preferred, FlightNumber+scheduled-time fallback — see FindArrivalAsync),
// then delegates to the same command handler the API's delete endpoint uses, so an external delete
// cascades to the linked turnaround flight, fires rules, and cleans up park positions
// exactly like an operator-initiated delete would.
public sealed class ArrivalFlightDeletedProcessor(IAeroFlightDbContext db, ICommandHandler<DeleteArrivalFlightCommand, bool> deleteHandler)
    : FlightInboundProcessor<FlightDeleted>(db)
{
    public override string MessageType => RoutingKeys.Inbound.ArrivalFlightDeleted;

    protected override async Task ApplyAsync(FlightDeleted m, CancellationToken ct)
    {
        var f = await FindArrivalAsync(m.Id, m.FlightNumber, m.Scheduled, ct);
        await deleteHandler.HandleAsync(new DeleteArrivalFlightCommand(f.Id, m.DeleteLinkedFlight), ct);
    }
}
