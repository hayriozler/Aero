namespace Aero.Flights.Contracts.Flights;

public sealed record OpsRoutingRequest(string AirportCode);

public sealed record OpsRoutingResponse(
    long Id,
    long FlightId,
    string AirportCode,
    int LegNumber);
