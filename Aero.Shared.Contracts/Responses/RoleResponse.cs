namespace Aero.Shared.Contracts.Responses;

public sealed record RoleResponse(
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    string CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy);
