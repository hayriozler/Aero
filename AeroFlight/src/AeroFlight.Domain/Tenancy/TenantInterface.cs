namespace AeroFlight.Domain.Tenancy;

// Informational registry only — enabling an interface here doesn't itself gate any behavior.
public static class InterfaceCodes
{
    public const string Fids = "FIDS";
    public const string Brs = "BRS";
}

public sealed class TenantInterface
{
    public string TenantId { get; private set; } = string.Empty;
    public string InterfaceCode { get; private set; } = string.Empty;
    public bool IsEnabled { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private TenantInterface() { }

    public static TenantInterface Create(string tenantId, string interfaceCode, bool isEnabled, DateTimeOffset now)
        => new()
        {
            TenantId = tenantId,
            InterfaceCode = interfaceCode,
            IsEnabled = isEnabled,
            CreatedAt = now,
        };

    public void SetEnabled(bool isEnabled, DateTimeOffset now)
    {
        IsEnabled = isEnabled;
        UpdatedAt = now;
    }
}
