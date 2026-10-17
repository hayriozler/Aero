using Aero.Flights.Contracts.Flights;

namespace Aero.Flights.Application.Flights.Commands;

public sealed record CreateArrivalFlightCommand(ArrivalCreateRequest Request);
