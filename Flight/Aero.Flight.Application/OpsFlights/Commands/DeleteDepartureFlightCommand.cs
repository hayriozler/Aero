namespace Aero.Flights.Application.OpsFlights.Commands;

public sealed record DeleteDepartureFlightCommand(long Id, bool DeleteLinkedFlight = false);
