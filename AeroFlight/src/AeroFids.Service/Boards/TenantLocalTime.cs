namespace AeroFids.Service.Boards;

internal static class TenantLocalTime
{
    public static string Format(DateTimeOffset utc, string tenantTimeZoneId) =>
        TimeZoneInfo.ConvertTime(utc, Resolve(tenantTimeZoneId)).ToString("HH:mm");

    public static string? FormatOrNull(DateTimeOffset? utc, string tenantTimeZoneId) =>
        utc is { } value ? Format(value, tenantTimeZoneId) : null;

    private static TimeZoneInfo Resolve(string tenantTimeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(tenantTimeZoneId);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return TimeZoneInfo.Utc;
        }
    }
}
