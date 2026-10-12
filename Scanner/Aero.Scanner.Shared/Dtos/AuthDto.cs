namespace Aero.Scanner.Shared.Dtos;

public sealed record LoginResponseDto(
    string Token,
    string RefreshToken,
    string Username,
    string DisplayName,
    IReadOnlyList<string> Roles,
    string CompanyCode,
    string CompanyName,
    string CompanyType,
    DateTime ExpiresAt);

public sealed record UserDto(
    int Id,
    string Username,
    string DisplayName,
    IReadOnlyList<string> Roles,
    string CompanyCode,
    string CompanyName,
    string CompanyType,
    DateTime? LastLoginAt);
