namespace Aero.Shared.Contracts.Dtos;

public sealed record UserDto(
    int Id,
    string Username,
    string DisplayName,
    IReadOnlyList<string> Roles,
    string CompanyCode,
    string CompanyName,
    string CompanyType,
    DateTime? LastLoginAt);
