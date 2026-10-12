using AeroFlight.Application.Flights.Commands;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Inbound;
using AeroFlight.Contracts.Topology;

namespace AeroFlight.Application.Flights.InboundProcessors;

public sealed class DepartureFlightUpdatedProcessor(IAeroFlightDbContext db, IFlightUpdateService updateService)
    : FlightInboundProcessor<DepartureFlightUpdated>(db)
{
    public override string MessageType => RoutingKeys.Inbound.DepartureFlightUpdated;

    protected override async Task ApplyAsync(DepartureFlightUpdated m, CancellationToken ct)
    {
        var f = await FindDepartureAsync(m.Id, m.FlightNumber, m.Std, ct);
        await updateService.PatchDepartureAsync(f, m, ct);
    }
}
