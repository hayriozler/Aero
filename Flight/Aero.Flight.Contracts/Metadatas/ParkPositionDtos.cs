namespace Aero.Flights.Contracts.Metadatas;

public sealed record ParkPositionRequest(
    string Code, string? TypeCode, string? Description, int? Capacity,
    decimal? Latitude, decimal? Longitude, decimal? Angle);

public sealed record ParkPositionResponse(
    string Code, string? TypeCode,
    string? Description, int? Capacity,
    decimal? Latitude, decimal? Longitude, decimal? Angle);
