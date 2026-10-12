using Aero.Scanner.Shared.Dtos;
using Aero.Scanner.Shared.Requests;

namespace Aero.Scanner.WebApp.Services;

public sealed class HhtApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<HandheldTerminalDto>> GetAllAsync(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<List<HandheldTerminalDto>>("/api/hht", ct) ?? [];

    public async Task<(bool Success, HandheldTerminalDto? Data, string? Error)> CreateAsync(
        CreateHandheldTerminalRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/hht", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<HandheldTerminalDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, HandheldTerminalDto? Data, string? Error)> UpdateAsync(
        int id, UpdateHandheldTerminalRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/hht/{id}", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<HandheldTerminalDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, HandheldTerminalDto? Data, string? Error)> AssignAsync(
        int id, AssignHandheldTerminalRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync($"/api/hht/{id}/assign", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<HandheldTerminalDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, HandheldTerminalDto? Data, string? Error)> UnassignAsync(
        int id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/hht/{id}/assign", ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<HandheldTerminalDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/hht/{id}", ct);
        return response.IsSuccessStatusCode;
    }
}
