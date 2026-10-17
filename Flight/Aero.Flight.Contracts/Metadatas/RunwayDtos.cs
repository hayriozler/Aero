namespace Aero.Flights.Contracts.Metadatas;

public sealed record RunwayRequest(string Code, string? Name, decimal? Length, decimal? Width);

public sealed record RunwayResponse(string Code, string? Name, decimal? Length, decimal? Width);
