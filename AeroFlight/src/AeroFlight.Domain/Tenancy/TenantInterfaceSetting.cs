namespace AeroFlight.Domain.Tenancy;

// Generic per-tenant, per-interface settings store — Key is dotted-prefixed by interface, e.g.
// "FIDS.ConnectionString", matching the AeroConfiguration naming convention. Value is stored
// encrypted for anything credential-bearing (see InterfaceSettingsService).
public static class InterfaceSettingKeys
{
    public const string FidsConnectionString = "FIDS.ConnectionString";
}

public sealed class TenantInterfaceSetting
{
    public int Id { get; private set; }
    public string TenantId { get; private set; } = string.Empty;
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    private TenantInterfaceSetting() { }

    public static TenantInterfaceSetting Create(string tenantId, string key, string value) => new()
    {
        TenantId = tenantId,
        Key = key,
        Value = value,
    };

    public void UpdateValue(string value) => Value = value;
}
