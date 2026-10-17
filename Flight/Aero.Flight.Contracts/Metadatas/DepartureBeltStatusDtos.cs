namespace Aero.Flights.Contracts.Metadatas;

public sealed record DepartureBeltStatusResponse(string Code, string Description, bool IsDefault, bool IsOpenValue, bool IsCloseValue);
