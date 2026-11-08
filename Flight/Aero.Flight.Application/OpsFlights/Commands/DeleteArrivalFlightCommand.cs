namespace Aero.Flights.Application.OpsFlights.Commands;

public sealed record DeleteArrivalFlightCommand(long Id, bool DeleteLinkedFlight = false);
