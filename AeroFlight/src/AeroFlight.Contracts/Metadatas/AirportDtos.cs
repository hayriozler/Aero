namespace AeroFlight.Contracts.Metadatas;

public sealed record AirportRequest(
    string Code, string? IataCode, string? IcaoCode,
    string? Name, string? DisplayName, string? CountryCode, string? TimezoneCode);

public sealed record AirportResponse(
    string Code, string? IataCode, string? IcaoCode,
    string? Name, string? DisplayName,
    string? CountryCode, string? TimezoneCode);
