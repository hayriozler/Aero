namespace Aero.Fids.Application.DisplayClients;

public sealed record DisplayClientResponse(
    long Id, string Name, string Type, string? ResourceCode, string? Location, string? BankName, int? BankPosition, bool IsActive);
