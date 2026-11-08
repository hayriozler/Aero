namespace Aero.Shared.Contracts.Dtos;

public sealed record RoleDto(
    string Name,
    string DisplayName,
    int UserCount,
    IReadOnlyList<PermissionDto> Permissions);
