namespace Aero.Shared.Contracts.Requests;

public sealed record UpdatePermissionRequest(
    string DisplayName,
    string Group,
    string? Description);
