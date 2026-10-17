using Aero.Flights.Domain.Common;
namespace Aero.Flights.Domain.Metadatas;

public sealed class HandlingService : AuditBase<string>
{
    public string? Description { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    private HandlingService() { }
    private HandlingService(string code) => SetId(code);
    public static HandlingService Create(string code) => new(code);

    public void Update(string unit, string? description)
    {
        Unit = unit;
        Description = description;
    }
}
