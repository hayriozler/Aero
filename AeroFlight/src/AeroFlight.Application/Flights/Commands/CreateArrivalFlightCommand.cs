using AeroFlight.Contracts.Flights;

namespace AeroFlight.Application.Flights.Commands;

public sealed record CreateArrivalFlightCommand(ArrivalCreateRequest Request);
