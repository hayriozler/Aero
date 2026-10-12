namespace AeroFlight.Domain.Metadatas;

public sealed class CountryType
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Severity { get; private set; }
    private CountryType() { }

}
