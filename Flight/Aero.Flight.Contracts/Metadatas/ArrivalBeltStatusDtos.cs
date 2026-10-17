namespace Aero.Flights.Contracts.Metadatas;

public sealed record ArrivalBeltStatusResponse(string Code, string Description, bool IsDefault, bool IsOpenValue, bool IsCloseValue);
