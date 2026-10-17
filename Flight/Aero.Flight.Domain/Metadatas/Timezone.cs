namespace Aero.Flights.Domain.Metadatas;

// Code is the IANA time zone id (e.g. "Europe/Istanbul") — the same identifier format
// TimeZoneInfo.FindSystemTimeZoneById expects cross-platform on .NET 6+.
public sealed class Timezone
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    private Timezone() { }
}
