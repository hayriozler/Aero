using Aero.Flights.Application.Flights.Commands;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Inbound;
using Aero.Flights.Contracts.Topology;

namespace Aero.Flights.Application.Flights.InboundProcessors;

public sealed class ArrivalFlightCreatedProcessor(IAeroFlightDbContext db, IFlightCreationService creationService)
    : FlightInboundProcessor<ArrivalFlightCreated>(db)
{
    public override string MessageType => RoutingKeys.Inbound.ArrivalFlightCreated;

    protected override async Task ApplyAsync(ArrivalFlightCreated m, CancellationToken ct)
    {
        var input = new ArrivalFlightCreationInput(
            m.AirlineCode, m.FlightNumber, m.Sta, m.OriginAirportCode,
            FlightTypeCode: m.FlightTypeCode,
            StatusCode: m.StatusCode,
            CallSign: m.Callsign,
            CodeshareNumber: m.CodeshareNumber,
            Codeshares: m.Codeshares?.Select(cs => (cs.AirlineCode, cs.FlightNumber)).ToList(),
            AircraftCode: m.AircraftCode,
            AircraftTypeCode: m.AircraftTypeCode,
            OperatingAirlineCode: m.OperatingAirlineCode,
            TerminalCode: m.TerminalCode,
            PaxTotal: m.PaxTotal);

        await creationService.CreateArrivalAsync(input, ct);
    }
}
