namespace Aero.Flights.Application.Flights.Commands;

public sealed record DeleteArrivalFlightCommand(long Id, bool DeleteLinkedFlight = false);
