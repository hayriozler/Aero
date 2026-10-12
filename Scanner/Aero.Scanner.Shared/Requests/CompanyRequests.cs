namespace Aero.Scanner.Shared.Requests;

public sealed record CreateCompanyRequest(
    string Code,
    string Name,
    string Type);

public sealed record UpdateCompanyRequest(
    string Code,
    string Name,
    string Type);
