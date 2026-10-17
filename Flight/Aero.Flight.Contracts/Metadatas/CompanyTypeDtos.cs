namespace Aero.Flights.Contracts.Metadatas;

public sealed record CompanyTypeRequest(string Code, string Description);

public sealed record CompanyTypeResponse(string Code, string Description);
