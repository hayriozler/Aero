using System.Security.Claims;

namespace Aero.Shared.Application.Managers;

public static class TimeZoneManager
{
    public static TimeZoneInfo Resolve(ClaimsPrincipal user)
    {
        var id = user.FindFirst("TimeZoneManager")?.Value;
        if (string.IsNullOrWhiteSpace(id))
            return TimeZoneInfo.Utc;

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.Utc;
        }
    }

    public static DateTimeOffset ToUtcOffset(DateTime localWallClock, TimeZoneInfo timeZone)
    {
        var unspecified = DateTime.SpecifyKind(localWallClock, DateTimeKind.Unspecified);
        var offset = timeZone.GetUtcOffset(unspecified);
        return new DateTimeOffset(unspecified, offset).ToUniversalTime();
    }

    public static DateTimeOffset ToLocal(DateTimeOffset utc, TimeZoneInfo timeZone) =>
        TimeZoneInfo.ConvertTime(utc, timeZone);
}
