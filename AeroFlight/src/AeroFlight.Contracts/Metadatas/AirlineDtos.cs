namespace AeroFlight.Contracts.Metadatas;

public sealed record AirlineRequest(
    string Code, string IataCode, string? IcaoCode,
    string? Callsign, string? Name, string? DisplayName,
    string? ParentAirlineCode, string? CountryCode);

public sealed record AirlineResponse(
    string Code, string IataCode, string? IcaoCode,
    string? FlightNumberCode, string? Callsign, string? Name, string? DisplayName,
    string? ParentAirlineCode, string? CountryCode);
