using AeroFlight.Application.Flights.Commands;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Inbound;
using AeroFlight.Contracts.Topology;

namespace AeroFlight.Application.Flights.InboundProcessors;

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
