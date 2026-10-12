namespace AeroFlight.Contracts.Metadatas;

public sealed record AirportImageResponse(int Id, string AirportCode, string ContentType, int SizeBytes, int DisplayOrder);
