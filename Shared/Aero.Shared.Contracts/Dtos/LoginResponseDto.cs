namespace Aero.Shared.Contracts.Dtos;

public sealed record LoginResponseDto(
    string Token,
    string RefreshToken,
    string Username,
    string? RealName,
    IReadOnlyList<string> Roles,
    DateTime ExpiresAt);

