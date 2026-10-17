using Aero.Flights.Domain.Common;

namespace Aero.Flights.Domain.Metadatas;

public sealed class Runway : AuditBase<string>
{
    public string? Name { get; private set; }
    public decimal? Length { get; private set; }
    public decimal? Width { get; private set; }

    private Runway() { }

    private Runway(string code, string? name)
    {
        SetId(code);
        Name = name;
    }

    public static Runway Create(string code, string? name = null) => new(code, name);

    public void Update(string? name, decimal? length, decimal? width)
    {
        Name = name;
        Length = length;
        Width = width;
    }
}
