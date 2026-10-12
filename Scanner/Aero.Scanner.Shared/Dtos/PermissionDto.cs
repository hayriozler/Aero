namespace Aero.Scanner.Shared.Dtos;

public sealed record PermissionDto(
    int Id,
    string Name,
    string DisplayName,
    string? Description,
    string Group);
