namespace Aero.Fids.Web.Boards;

internal static class TenantLocalTime
{
    public static string Format(DateTimeOffset utc, string TimeZoneManagerId) =>
        TimeZoneInfo.ConvertTime(utc, Resolve(TimeZoneManagerId)).ToString("HH:mm");

    public static string? FormatOrNull(DateTimeOffset? utc, string TimeZoneManagerId) =>
        utc is { } value ? Format(value, TimeZoneManagerId) : null;

    private static TimeZoneInfo Resolve(string TimeZoneManagerId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneManagerId);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return TimeZoneInfo.Utc;
        }
    }
}
