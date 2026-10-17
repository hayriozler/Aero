using MudBlazor;

namespace Aero.Flights.Web.Services;

public static class TimeInputMask
{
    public static readonly IMask Mask = new PatternMask("00:00");
}
