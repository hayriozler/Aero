namespace Aero.Shared.Domain;

public class ClientApp
{
    public string Name { get; private set; } = string.Empty;
    public string TenantId { get; private set; } = string.Empty;
    public string Secret { get; private set; } = string.Empty;
    public DateTimeOffset BeginDateTime { get; private set; }
    public DateTimeOffset? EndDateTime { get; private set; }

    private ClientApp() { }

    private ClientApp(string name, string tenantId, string secret, DateTimeOffset? beginDateTime, DateTimeOffset? endDateTime)
    {
        Name = name;
        TenantId = tenantId;
        Secret = secret;
        BeginDateTime = beginDateTime ?? DateTimeOffset.UtcNow;
        EndDateTime = endDateTime;
    }

    public static ClientApp Create(string name, string tenantId, string secret, DateTimeOffset? beginDateTime, DateTimeOffset? endDateTime)
        => new(name, tenantId, secret, beginDateTime, endDateTime);

    public void ChangeSecret(string secret, DateTimeOffset? endDateTime)
    {
        Secret = secret;
        EndDateTime = endDateTime;
    }
}
