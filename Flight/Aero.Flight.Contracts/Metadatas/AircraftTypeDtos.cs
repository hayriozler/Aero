namespace Aero.Flights.Contracts.Metadatas;

public sealed record AircraftTypeRequest(
    string Code, string IataCode, string? IataSubCode, string? IcaoCode, string? SizeCategory,
    string? NoiseCategory, string? Manufacturer, string? Model, decimal? Mtow,
    decimal? LandingWeight, int? AvgSeatCapacity, bool IsBridgeRequired);


public sealed record AircraftTypeResponse(
    string Code, string? IataCode, string? IataSubCode, string? IcaoCode, string? SizeCategory,
    string? NoiseCategory, string? Manufacturer, string? Model, decimal? Mtow,
    decimal? LandingWeight, int? AvgSeatCapacity, bool IsBridgeRequired);
