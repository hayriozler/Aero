namespace Aero.Scanner.Shared.Requests;

public sealed record CreateRoleRequest(
    string Name,
    string DisplayName,
    List<int> PermissionIds);

public sealed record UpdateRoleRequest(
    string DisplayName,
    List<int> PermissionIds);
