namespace AeroFlight.Domain.Metadatas;

// Well-known FlightStatus codes referenced directly in code (cancellation, the default/seed
// status, and the "has this flight already left the gate" check) — everything else about a
// flight's status is free-form, sourced from the FlightStatus table itself, not hardcoded here.
public static class FlightStatusCodes
{
    public const string Scheduled = "SKD";
    public const string Cancelled = "XLD";
    public const string OffBlock = "OFB";
    public const string Departed = "OFF";
}
