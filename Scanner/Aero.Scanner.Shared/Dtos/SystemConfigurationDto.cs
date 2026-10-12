namespace Aero.Scanner.Shared.Dtos;

public sealed record SystemConfigurationDto(
    string Key,
    string Value,
    string? Description);
