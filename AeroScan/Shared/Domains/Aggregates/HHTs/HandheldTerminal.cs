using Domain.Aggregates.Companies;
using Domain.Common;

namespace Domain.Aggregates.HHTs;

public sealed class HandheldTerminal : AuditableEntity<int>
{
    /// <summary>Unique device identifier set at registration, e.g. "HHT-001"</summary>
    public string DeviceId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? SerialNumber { get; private set; }
    public string? Model { get; private set; }
    public string? AssignedCompanyCode { get; private set; }
    public DateTime? AssignedAt { get; private set; }

    public Company? AssignedCompany { get; private set; }

    private HandheldTerminal() { }

    public static HandheldTerminal Create(string deviceId, string name, string? serialNumber, string? model)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new HandheldTerminal
        {
            DeviceId     = deviceId.ToUpperInvariant().Trim(),
            Name         = name.Trim(),
            SerialNumber = serialNumber?.Trim(),
            Model        = model?.Trim()
        };
    }

    public void Update(string name, string? serialNumber, string? model)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name         = name.Trim();
        SerialNumber = serialNumber?.Trim();
        Model        = model?.Trim();
    }

    public void AssignToCompany(string companyCode)
    {
        AssignedCompanyCode = companyCode;
        AssignedAt        = DateTime.UtcNow;
    }

    public void Unassign()
    {
        AssignedCompanyCode = null;
        AssignedAt        = null;
    }
}
