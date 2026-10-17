namespace Aero.Flights.Contracts.Metadatas;

public sealed record FlightStatusResponse(
    string Code, string? Description, bool IsUpdatable);
