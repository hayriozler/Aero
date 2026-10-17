using Aero.Flights.Application.Common;
using Aero.Flights.Application.Flights.Queries;

namespace Aero.Flights.Application.Flights.Commands;

public sealed class CreateArrivalFlightCommandHandler(IFlightCreationService creationService)
    : ICommandHandler<CreateArrivalFlightCommand, CreateFlightResult>
{
    public async Task<CreateFlightResult> HandleAsync(CreateArrivalFlightCommand command, CancellationToken ct)
    {
        var r = command.Request;
        var input = new ArrivalFlightCreationInput(
            r.AirlineCode, r.FlightNumber, r.Sta, r.OriginAirportCode,
            ExtraRoutings: string.IsNullOrWhiteSpace(r.PreviousAirportCode) ? null : [(r.PreviousAirportCode, 1)],
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

        var (outcome, flight) = await creationService.CreateArrivalAsync(input, ct);
        return new CreateFlightResult(outcome, flight is null ? null : FlightResponseFactory.FromArrival(flight));
    }
}
