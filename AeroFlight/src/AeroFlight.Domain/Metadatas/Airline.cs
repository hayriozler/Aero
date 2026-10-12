using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Metadatas;

public sealed class Airline : AuditBase<string>
{
    public string IataCode { get; private set; } = string.Empty;
    public string? IcaoCode { get; private set; }
    public string? FlightNumberCode { get; private set; }
    public string? Callsign { get; private set; }
    public string? Name { get; private set; }
    public string? DisplayName { get; private set; }
    public string? ParentAirlineCode { get; private set; }
    public string? CountryCode { get; private set; }

    private Airline() { }

    private Airline(string code, string iataCode, string? name, string? displayName)
    {
        SetId(code);
        IataCode = iataCode;
        Name = name;
        DisplayName = displayName;
    }

    public static Airline Create(string code, string iataCode, string? name = null, string? displayName = null)
        => new(code, iataCode, name, displayName);

    public void Update(string? icaoCode, string? callsign,
        string? name, string? displayName, string? parentAirlineCode, string? countryCode)
    {
        IcaoCode = icaoCode;
        Callsign = callsign;
        Name = name;
        DisplayName = displayName;
        ParentAirlineCode = parentAirlineCode;
        CountryCode = countryCode;
    }
}
