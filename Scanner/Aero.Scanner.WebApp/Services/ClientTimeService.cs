using Microsoft.JSInterop;

namespace Aero.Scanner.WebApp.Services;

public sealed class ClientTimeService
{
    private TimeSpan _offset = TimeSpan.Zero;
    private bool _loaded;

    public async Task LoadAsync(IJSRuntime js)
    {
        if (_loaded) return;
        try
        {
            var minutes = await js.InvokeAsync<int>("brs.getUtcOffsetMinutes");
            _offset = TimeSpan.FromMinutes(minutes);
        }
        catch
        {
            // Fall back to UTC if JS interop is unavailable (pre-render / SSR phase)
            _offset = TimeSpan.Zero;
        }
        _loaded = true;
    }

    /// <summary>Converts a UTC DateTime to the client's local time.</summary>
    public DateTime ToLocal(DateTime utcTime)
    {
        var utc = utcTime.Kind == DateTimeKind.Local
            ? utcTime.ToUniversalTime()
            : DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
        return utc.Add(_offset);
    }

    /// <summary>Formats a nullable UTC DateTime as a local time string.</summary>
    public string Format(DateTime? utcTime, string format = "yyyy-MM-dd HH:mm") =>
        utcTime.HasValue ? ToLocal(utcTime.Value).ToString(format) : "--";

    /// <summary>Formats a UTC DateTime as a local time string.</summary>
    public string Format(DateTime utcTime, string format = "yyyy-MM-dd HH:mm") =>
        ToLocal(utcTime).ToString(format);

    /// <summary>Short time-only format (HH:mm).</summary>
    public string FormatTime(DateTime? utcTime) => Format(utcTime, "HH:mm");

    /// <summary>Date-only format.</summary>
    public string FormatDate(DateTime? utcTime) => Format(utcTime, "yyyy-MM-dd");
}
