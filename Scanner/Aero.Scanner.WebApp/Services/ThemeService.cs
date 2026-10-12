using Microsoft.JSInterop;

namespace Aero.Scanner.WebApp.Services;

/// <summary>
/// Scoped — one instance per Blazor Server circuit.
/// Persists the selected theme in localStorage and toggles the "dark" class on the html element.
/// </summary>
public sealed class ThemeService
{
    public bool IsDark { get; private set; } = true;

    public event Action? OnThemeChanged;

    public async Task InitAsync(IJSRuntime js)
    {
        var stored = await js.InvokeAsync<string?>("brs.getTheme");
        IsDark = stored != "light";
        await js.InvokeVoidAsync("brs.applyTheme", IsDark);
    }

    public async Task ToggleAsync(IJSRuntime js)
    {
        IsDark = !IsDark;
        await js.InvokeVoidAsync("brs.applyTheme", IsDark);
        OnThemeChanged?.Invoke();
    }
}
