using Aero.Flights.Application.Interfaces;
using Aero.Flights.Application.OpsFlights.Commands;
using Aero.Flights.Contracts.Inbound;
using Aero.Flights.Contracts.Topology;

namespace Aero.Flights.Application.OpsFlights.InboundProcessors;

public sealed class ArrivalFlightUpdatedProcessor(IAeroFlightDbContext db, IFlightUpdateService updateService)
    : FlightInboundProcessor<ArrivalFlightUpdated>(db)
{
    public override string MessageType => RoutingKeys.Inbound.ArrivalFlightUpdated;

    protected override async Task ApplyAsync(ArrivalFlightUpdated m, CancellationToken ct)
    {
        var f = await FindArrivalAsync(m.Id, m.FlightNumber, m.Sta, ct);
        await updateService.PatchArrivalAsync(f, m, ct);
    }
}
