using System.Security.Claims;

namespace AeroFlight.Web.Services;

public static class TenantTimeZone
{
    public static TimeZoneInfo Resolve(ClaimsPrincipal user)
    {
        var id = user.FindFirst("tenantTimeZone")?.Value;
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

    // localWallClock is the date+time as picked by the user, meant to represent that tenant's local time.
    // Normalized to Offset=0 before returning — Npgsql rejects any non-zero offset for "timestamp with time zone".
    public static DateTimeOffset ToUtcOffset(DateTime localWallClock, TimeZoneInfo timeZone)
    {
        var unspecified = DateTime.SpecifyKind(localWallClock, DateTimeKind.Unspecified);
        var offset = timeZone.GetUtcOffset(unspecified);
        return new DateTimeOffset(unspecified, offset).ToUniversalTime();
    }

    public static DateTimeOffset ToLocal(DateTimeOffset utc, TimeZoneInfo timeZone) =>
        TimeZoneInfo.ConvertTime(utc, timeZone);
}
