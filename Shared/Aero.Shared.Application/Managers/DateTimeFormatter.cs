using System.Security.Claims;

namespace Aero.Shared.Application.Managers;

public static class DateTimeFormatter
{
    public const string DefaultDateFormat = "dd/MM/yyyy";
    public const string DefaultTimeFormat = "HH:mm";

    public static string ResolveDateFormat(ClaimsPrincipal user) =>
        user.FindFirst("tenantDateFormat")?.Value is { Length: > 0 } value ? value : DefaultDateFormat;

    public static string ResolveTimeFormat(ClaimsPrincipal user) =>
        user.FindFirst("tenantTimeFormat")?.Value is { Length: > 0 } value ? value : DefaultTimeFormat;

    public static string ResolveDateTimeFormat(ClaimsPrincipal user) =>
        $"{ResolveDateFormat(user)} {ResolveTimeFormat(user)}";
}
