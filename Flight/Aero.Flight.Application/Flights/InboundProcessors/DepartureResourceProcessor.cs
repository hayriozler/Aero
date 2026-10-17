using Aero.Flights.Application.Flights.Resources;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Inbound;
using Aero.Flights.Contracts.Topology;
using Aero.Flights.Domain.Ops.Resources;

namespace Aero.Flights.Application.Flights.InboundProcessors;

public sealed class DepartureResourceProcessor(
    IAeroFlightDbContext db,
    IFlightResourceService<OpsDepartureGate> gateService,
    IFlightResourceService<OpsDepartureBelt> beltService,
    IFlightResourceService<OpsCounter> counterService)
    : FlightInboundProcessor<FlightResourceMessage>(db)
{
    public override string MessageType => RoutingKeys.Inbound.DepartureResourceChanged;

    protected override async Task ApplyAsync(FlightResourceMessage m, CancellationToken ct)
    {
        var flight = await FindDepartureAsync(m.Id, m.FlightNumber, m.Scheduled, ct);

        switch (m.ResourceKind)
        {
            case "Gate":
                await ApplyResourceMessageAsync(gateService, flight.Id, m.ResourceKind, m.Resources, ct);
                break;
            case "Belt":
                await ApplyResourceMessageAsync(beltService, flight.Id, m.ResourceKind, m.Resources, ct);
                break;
            case "Counter":
                await ApplyResourceMessageAsync(counterService, flight.Id, m.ResourceKind, m.Resources, ct);
                break;
            default:
                throw new InvalidOperationException($"Unsupported departure resource kind '{m.ResourceKind}'.");
        }
    }
}
