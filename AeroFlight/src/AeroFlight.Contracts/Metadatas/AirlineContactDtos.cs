namespace AeroFlight.Contracts.Metadatas;

public sealed record AirlineContactRequest(string ContactType, string Value);

public sealed record AirlineContactResponse(int Id, string AirlineCode, string ContactType, string Value);
