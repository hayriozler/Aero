namespace AeroFlight.Contracts.Fids;

// Shared by AeroFlight and AeroFids so "arrival"/"departure" normalization can't drift apart.
public static class FlightTypes
{
    public const string Arrival = "Arrival";
    public const string Departure = "Departure";

    public static string? Normalize(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        "arrival" => Arrival,
        "departure" => Departure,
        _ => null,
    };
}
