using MudBlazor;

namespace Aero.Flights.Web.Services.Flights;

// Single place that turns a flight status code into a chip color. The code->color mapping itself
// lives in AeroConfiguration ("FlightStatus.Color.{code}", falls back to "FlightStatus.Color.Default")
// — fetched once via IFlightsClient.GetFlightStatusColorsAsync and passed in here, never hardcoded.
public static class FlightStatusColors
{
    public static Color Resolve(IReadOnlyDictionary<string, string> colors, string status)
    {
        if (colors.TryGetValue(status, out var name) || colors.TryGetValue("Default", out name))
        {
            if (Enum.TryParse<Color>(name, ignoreCase: true, out var color))
                return color;
        }

        return Color.Success;
    }
}
