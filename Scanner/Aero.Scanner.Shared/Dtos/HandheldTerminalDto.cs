namespace Aero.Scanner.Shared.Dtos;

public sealed record HandheldTerminalDto(
    int Id,
    string DeviceId,
    string Name,
    string? SerialNumber,
    string? Model,
    string? AssignedCompanyCode,
    string? AssignedCompanyName,
    DateTime? AssignedAt,
    DateTime CreatedAt);
