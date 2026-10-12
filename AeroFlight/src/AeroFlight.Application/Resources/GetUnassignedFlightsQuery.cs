namespace AeroFlight.Application.Resources;

public sealed record GetUnassignedFlightsQuery(string ResourceType, DateOnly FromDate, DateOnly ToDate);

public sealed record UnassignedFlightItem(long FlightId, string Direction, string FlightCode, DateTimeOffset ScheduledTime);
