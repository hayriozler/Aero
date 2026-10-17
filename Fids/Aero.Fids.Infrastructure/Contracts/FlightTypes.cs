namespace Aero.Fids.Infrastructure.Contracts;

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