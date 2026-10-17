namespace Aero.Shared.Contracts.Response;

public sealed record UserResponse(
    int Id,
    string Username,
    string? RealName,
    bool IsActive,
    DateTimeOffset? ExpirationDate,
    IReadOnlyList<string> Roles,
    DateTimeOffset CreatedAt,
    string CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy);
