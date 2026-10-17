namespace Aero.Shared.Contracts.Requests;

public sealed record UpdateRoleRequest(
    string DisplayName,
    List<int> PermissionIds);
