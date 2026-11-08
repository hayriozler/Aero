namespace Aero.Shared.Contracts.Requests;

public sealed record CreatePermissionRequest(
    string Name,
    string DisplayName,
    string Group,
    string? Description);
