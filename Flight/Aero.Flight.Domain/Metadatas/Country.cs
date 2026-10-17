using Aero.Flights.Domain.Common;
namespace Aero.Flights.Domain.Metadatas;

public sealed class Country : AuditBase<string>
{
    public string Name { get; private set; } = string.Empty;
    public string CountryTypeCode { get; private set; } = string.Empty;
    public CountryType CountryType { get; private set; } = null!;
    public string? RegionCode { get; private set; }

    private Country() { }

    private Country(string code, string name, string countryTypeCode, string? regionCode)
    {
        SetId(code);
        Name = name;
        CountryTypeCode = countryTypeCode;
        RegionCode = regionCode;
    }

    public static Country Create(string code, string name, string countryTypeCode, string? regionCode = null)
        => new(code, name, countryTypeCode, regionCode);

    public void Update(string name, string countryTypeCode, string? regionCode = null)
    {
        Name = name;
        CountryTypeCode = countryTypeCode;
        RegionCode = regionCode;
    }
}
