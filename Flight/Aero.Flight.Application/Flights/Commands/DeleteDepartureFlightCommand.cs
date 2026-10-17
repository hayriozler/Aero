namespace Aero.Flights.Application.Flights.Commands;

public sealed record DeleteDepartureFlightCommand(long Id, bool DeleteLinkedFlight = false);
