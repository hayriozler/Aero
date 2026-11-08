namespace Aero.Fids.Application.DisplayClients;

public sealed record DisplayClientRequest(
    string Name, string Type, string? ResourceCode, string? Location, string? BankName, int? BankPosition, bool IsActive = true);
