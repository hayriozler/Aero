namespace Aero.Scanner.Shared.Dtos;

public sealed record CompanyDto(
    string Code,
    string Name,
    string Type,
    DateTime CreatedAt);
