using AeroFlight.Application.Flights.Commands;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Inbound;
using AeroFlight.Contracts.Topology;

namespace AeroFlight.Application.Flights.InboundProcessors;

public sealed class DepartureFlightCreatedProcessor(IAeroFlightDbContext db, IFlightCreationService creationService)
    : FlightInboundProcessor<DepartureFlightCreated>(db)
{
    public override string MessageType => RoutingKeys.Inbound.DepartureFlightCreated;

    protected override async Task ApplyAsync(DepartureFlightCreated m, CancellationToken ct)
    {
        var input = new DepartureFlightCreationInput(
            m.AirlineCode, m.FlightNumber, m.Std, m.DestinationAirportCode,
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

        await creationService.CreateDepartureAsync(input, ct);
    }
}
