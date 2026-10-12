namespace AeroFlight.Application.Flights.Commands;

public sealed record DeleteArrivalFlightCommand(long Id, bool DeleteLinkedFlight = false);
