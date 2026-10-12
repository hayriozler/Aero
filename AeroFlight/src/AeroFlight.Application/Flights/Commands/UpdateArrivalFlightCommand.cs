using AeroFlight.Contracts.Flights;

namespace AeroFlight.Application.Flights.Commands;

public sealed record UpdateArrivalFlightCommand(long Id, UpdateArrivalFlightRequest Request);
