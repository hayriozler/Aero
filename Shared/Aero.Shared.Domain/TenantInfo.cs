namespace Aero.Shared.Domain;

public sealed record TenantInfo(string Id, string Name, string ConnectionString, string HomeAirportCode, bool IsActive);
