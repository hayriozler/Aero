namespace Aero.Scanner.Shared.Requests;

public sealed record CreatePermissionRequest(
    string Name,
    string DisplayName,
    string Group,
    string? Description);

public sealed record UpdatePermissionRequest(
    string DisplayName,
    string Group,
    string? Description);
