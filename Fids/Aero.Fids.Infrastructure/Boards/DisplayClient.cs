namespace Aero.Fids.Infrastructure.Boards;

public sealed class DisplayClient
{
    public long Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string? ResourceCode { get; private set; }
    public string? Location { get; private set; }
    public string? BankName { get; private set; }
    public int? BankPosition { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private DisplayClient() { }

    public static DisplayClient Create(
        string name, string type, string? resourceCode, string? location, string? bankName, int? bankPosition, DateTimeOffset now) => new()
        {
            Name = name,
            Type = type,
            ResourceCode = resourceCode,
            Location = location,
            BankName = bankName,
            BankPosition = bankPosition,
            IsActive = true,
            CreatedAt = now,
        };

    public void Update(
        string name, string type, string? resourceCode, string? location, string? bankName, int? bankPosition, bool isActive, DateTimeOffset now)
    {
        Name = name;
        Type = type;
        ResourceCode = resourceCode;
        Location = location;
        BankName = bankName;
        BankPosition = bankPosition;
        IsActive = isActive;
        UpdatedAt = now;
    }
}
