namespace AeroFlight.Application.Flights.Commands;

public sealed record DeleteDepartureFlightCommand(long Id, bool DeleteLinkedFlight = false);
