using MudBlazor;

namespace Aero.Flights.Web.Services;

// Lets the user type "1930" and have it auto-format to "19:30" as they type, instead of requiring
// the colon — shared by every TimeMaskTextField so behavior stays consistent across screens.
public static class TimeInputMask
{
    public static readonly IMask Mask = new PatternMask("00:00");
}
