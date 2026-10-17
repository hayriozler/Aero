namespace Aero.Flights.Contracts.Metadatas;

public sealed record AirlineLogoResponse(int Id, string AirlineCode, string Name, string ContentType, int SizeBytes);

public sealed record AirlineLogoRenameRequest(string Name);
