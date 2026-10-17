namespace Aero.Shared.Domain;

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
