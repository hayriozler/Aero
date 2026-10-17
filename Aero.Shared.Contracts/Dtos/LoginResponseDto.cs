namespace Aero.Shared.Contracts.Dtos;

public sealed record LoginResponseDto(
    string Token,
    string RefreshToken,
    string Username,
    string DisplayName,
    IReadOnlyList<string> Roles,
    DateTime ExpiresAt);

