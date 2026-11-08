namespace Aero.Shared.Contracts.Requests;

public sealed record CreateRoleRequest(
    string Name,
    string DisplayName,
    List<int> PermissionIds);
