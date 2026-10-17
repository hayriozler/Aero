using Aero.Flights.Application.Flights.Commands;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Inbound;
using Aero.Flights.Contracts.Topology;

namespace Aero.Flights.Application.Flights.InboundProcessors;

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
