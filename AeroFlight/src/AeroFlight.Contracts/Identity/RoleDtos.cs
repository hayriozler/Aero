namespace AeroFlight.Contracts.Identity;

public sealed record RoleResponse(
    int Id,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    string CreatedBy,
    DateTimeOffset? UpdatedAt,
    string? UpdatedBy);
