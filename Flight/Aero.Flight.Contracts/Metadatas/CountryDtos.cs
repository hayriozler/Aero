namespace Aero.Flights.Contracts.Metadatas;

public sealed record CountryRequest(string Code, string Name, string CountryTypeCode);

public sealed record CountryResponse(string Code, string Name, string? CountryTypeCode);
