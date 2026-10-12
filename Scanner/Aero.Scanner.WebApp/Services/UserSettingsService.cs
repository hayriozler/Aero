using Microsoft.JSInterop;

namespace Aero.Scanner.WebApp.Services;

/// <summary>
/// Scoped — persists user preferences in localStorage.
/// Manages the auto-refresh toggle and refresh interval.
/// </summary>
public sealed class UserSettingsService
{
    public static readonly int[] RefreshOptions = [5, 10, 20, 30, 60];

    public int  RefreshIntervalSeconds { get; private set; } = 30;
    public bool AutoRefreshEnabled     { get; private set; } = true;

    public event Action? OnSettingsChanged;

    public async Task LoadAsync(IJSRuntime js)
    {
        var interval = await js.InvokeAsync<string?>("brs.getStorageItem", "brs-refresh-interval");
        if (int.TryParse(interval, out var s) && RefreshOptions.Contains(s))
            RefreshIntervalSeconds = s;

        var auto = await js.InvokeAsync<string?>("brs.getStorageItem", "brs-auto-refresh");
        AutoRefreshEnabled = auto != "false";
    }

    public async Task SetRefreshIntervalAsync(IJSRuntime js, int seconds)
    {
        RefreshIntervalSeconds = seconds;
        await js.InvokeVoidAsync("brs.setStorageItem", "brs-refresh-interval", seconds.ToString());
        OnSettingsChanged?.Invoke();
    }

    public async Task SetAutoRefreshAsync(IJSRuntime js, bool enabled)
    {
        AutoRefreshEnabled = enabled;
        await js.InvokeVoidAsync("brs.setStorageItem", "brs-auto-refresh", enabled ? "true" : "false");
        OnSettingsChanged?.Invoke();
    }
}
