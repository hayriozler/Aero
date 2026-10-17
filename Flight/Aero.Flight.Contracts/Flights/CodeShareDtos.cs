namespace Aero.Flights.Contracts.Flights;

public sealed record CodeShareRequest(
    string AirlineCode,
    string FlightNumber);

public sealed record CodeShareResponse(
    long Id,
    long FlightId,
    string AirlineCode,
    string FlightNumber);
