namespace Aero.Fids.Application;

public sealed record ConnectedClientInfo(
    string TenantId, string Name, string Type,
    string? Location, string? BankName, int? BankPosition, DateTimeOffset ConnectedAt);
