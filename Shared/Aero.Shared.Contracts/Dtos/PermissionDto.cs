namespace Aero.Shared.Contracts.Dtos;

public sealed record PermissionDto(
    int Id,
    string Name,
    string DisplayName,
    string? Description,
    string Group);
