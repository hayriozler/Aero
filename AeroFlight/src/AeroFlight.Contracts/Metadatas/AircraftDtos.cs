namespace AeroFlight.Contracts.Metadatas;

public sealed record AircraftRequest(
    string Code, string AircraftTypeCode, string? AirlineCode, string? Owner,
    string? SizeCategory, string? NoiseCategory,
    decimal? BillWeight, int? SeatCapacity);


public sealed record AircraftResponse(
    string Code, string AircraftTypeCode, string? AirlineCode, string? Owner,
    string? SizeCategory, string? NoiseCategory, decimal? BillWeight, int? SeatCapacity);
