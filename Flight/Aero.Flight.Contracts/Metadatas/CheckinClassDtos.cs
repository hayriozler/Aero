namespace Aero.Flights.Contracts.Metadatas;

public sealed record CheckinClassRequest(string Code, string Description);

public sealed record CheckinClassResponse(string Code, string Description);
