namespace Aero.Flights.Contracts.Metadatas;

public sealed record RemarkRequest(
    string Code, string Description);

public sealed record RemarkResponse(string Code, string Description);
