namespace AeroFlight.Domain.Tenancy;

public sealed class Tenant
{
    public string Id { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string ConnectionStringProtected { get; private set; } = string.Empty;
    public string HomeAirportCode { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private Tenant() { }

    public static Tenant Create(string id, string name, string connectionStringProtected,
        string homeAirportCode, bool isActive, DateTimeOffset now)
        => new()
        {
            Id = id,
            Name = name,
            ConnectionStringProtected = connectionStringProtected,
            HomeAirportCode = homeAirportCode,
            IsActive = isActive,
            CreatedAt = now,
        };

    public void Update(string name, string connectionStringProtected, string homeAirportCode,
        bool isActive, DateTimeOffset now)
    {
        Name = name;
        ConnectionStringProtected = connectionStringProtected;
        HomeAirportCode = homeAirportCode;
        IsActive = isActive;
        UpdatedAt = now;
    }
}
