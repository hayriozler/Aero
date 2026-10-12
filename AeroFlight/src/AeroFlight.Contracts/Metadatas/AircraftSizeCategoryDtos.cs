namespace AeroFlight.Contracts.Metadatas;

public sealed record AircraftSizeCategoryRequest(string Code, string Description);

public sealed record AircraftSizeCategoryResponse(string Code, string Description);
