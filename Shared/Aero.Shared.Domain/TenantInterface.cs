namespace Aero.Shared.Domain;

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
