namespace AeroFlight.Contracts.Flights;

public sealed record DelayRequest(
    string DelayCode,
    decimal? DelayMinutes,
    string? Comment);

public sealed record DelayResponse(
    long Id,
    long FlightId,
    string DelayCode,
    decimal? DelayMinutes,
    string? Comment);
