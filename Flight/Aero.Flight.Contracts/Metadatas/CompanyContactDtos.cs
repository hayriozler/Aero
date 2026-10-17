namespace Aero.Flights.Contracts.Metadatas;

public sealed record CompanyContactRequest(string ContactType, string Value);

public sealed record CompanyContactResponse(int Id, string CompanyCode, string ContactType, string Value);
