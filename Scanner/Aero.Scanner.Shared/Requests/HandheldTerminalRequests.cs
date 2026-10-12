namespace Aero.Scanner.Shared.Requests;

public sealed record CreateHandheldTerminalRequest(
    string DeviceId,
    string Name,
    string? SerialNumber,
    string? Model);

public sealed record UpdateHandheldTerminalRequest(
    string Name,
    string? SerialNumber,
    string? Model);

public sealed record AssignHandheldTerminalRequest(string HandlingCompanyCode);
