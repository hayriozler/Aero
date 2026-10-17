using Aero.Flights.Domain.Common;
namespace Aero.Flights.Domain.Metadatas;

public class AircraftSizeCategory : AuditBase<string>
{
    public string Description { get; private set; } = string.Empty;
    private AircraftSizeCategory() { }

    private AircraftSizeCategory(string code, string description)
    {
        SetId(code);
        Description = description;
    }

    public static AircraftSizeCategory Create(string code, string description) => new(code, description);

    public void Update(string description) => Description = description;
}
