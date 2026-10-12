using AeroFlight.Application.Flights.Resources;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Inbound;
using AeroFlight.Contracts.Topology;
using AeroFlight.Domain.Ops.Resources;

namespace AeroFlight.Application.Flights.InboundProcessors;

public sealed class ArrivalResourceProcessor(
    IAeroFlightDbContext db, IFlightResourceService<OpsArrivalGate> gateService, IFlightResourceService<OpsArrivalBelt> beltService)
    : FlightInboundProcessor<FlightResourceMessage>(db)
{
    public override string MessageType => RoutingKeys.Inbound.ArrivalResourceChanged;

    protected override async Task ApplyAsync(FlightResourceMessage m, CancellationToken ct)
    {
        var flight = await FindArrivalAsync(m.Id, m.FlightNumber, m.Scheduled, ct);

        switch (m.ResourceKind)
        {
            case "Gate":
                await ApplyResourceMessageAsync(gateService, flight.Id, m.ResourceKind, m.Resources, ct);
                break;
            case "Belt":
                await ApplyResourceMessageAsync(beltService, flight.Id, m.ResourceKind, m.Resources, ct);
                break;
            default:
                throw new InvalidOperationException($"Unsupported arrival resource kind '{m.ResourceKind}'.");
        }
    }
}
