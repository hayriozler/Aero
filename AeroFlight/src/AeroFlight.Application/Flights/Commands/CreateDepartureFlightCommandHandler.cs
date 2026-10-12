using AeroFlight.Application.Common;
using AeroFlight.Application.Flights.Queries;

namespace AeroFlight.Application.Flights.Commands;

public sealed class CreateDepartureFlightCommandHandler(IFlightCreationService creationService)
    : ICommandHandler<CreateDepartureFlightCommand, CreateFlightResult>
{
    public async Task<CreateFlightResult> HandleAsync(CreateDepartureFlightCommand command, CancellationToken ct)
    {
        var r = command.Request;
        var input = new DepartureFlightCreationInput(
            r.AirlineCode, r.FlightNumber, r.Std, r.DestinationAirportCode,
            ExtraRoutings: string.IsNullOrWhiteSpace(r.NextAirportCode) ? null : [(r.NextAirportCode, 1)],
            FlightTypeCode: r.FlightTypeCode,
            CallSign: r.CallSign,
            CodeshareNumber: r.CodeshareNumber,
            AircraftCode: r.AircraftCode,
            AircraftTypeCode: r.AircraftTypeCode,
            OperatingAirlineCode: r.OperatingAirlineCode,
            TerminalCode: r.TerminalCode,
            HandlingAgentCode: r.HandlingAgentCode,
            Comment: r.Comment,
            PaxTotal: r.PaxTotal);

        var (outcome, flight) = await creationService.CreateDepartureAsync(input, ct);
        return new CreateFlightResult(outcome, flight is null ? null : FlightResponseFactory.FromDeparture(flight));
    }
}
