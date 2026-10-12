using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Metadatas;

public sealed class Airport : AuditBase<string>
{
    public string? IataCode { get; private set; }
    public string? IcaoCode { get; private set; }
    public string? Name { get; private set; }
    public string? DisplayName { get; private set; }
    public string? CountryCode { get; private set; }
    public string? TimezoneCode { get; private set; }
    private Airport() { }

    private Airport(string code, string? iataCode, string? name, string? displayName)
    {
        SetId(code);
        IataCode = iataCode;
        Name = name;
        DisplayName = displayName;
    }

    public static Airport Create(string code, string? iataCode = null, string? name = null, string? displayName = null)
        => new(code, iataCode, name, displayName);

    public void Update(string? iataCode, string? icaoCode,
        string? name, string? displayName,
        string? countryCode, string? timezoneCode)
    {
        IataCode = iataCode;
        IcaoCode = icaoCode;
        Name = name;
        DisplayName = displayName;
        CountryCode = countryCode;
        TimezoneCode = timezoneCode;
    }
}
