namespace Aero.Shared.Contracts.Requests;

public sealed record SetPermissionsRequest(IReadOnlyList<string> PermissionCodes);