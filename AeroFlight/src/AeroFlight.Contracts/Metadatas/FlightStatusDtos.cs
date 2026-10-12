namespace AeroFlight.Contracts.Metadatas;

public sealed record FlightStatusResponse(
    string Code, string? Description, bool IsUpdatable);
