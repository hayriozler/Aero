using Aero.Scanner.Shared.Dtos;
using Aero.Scanner.Shared.Requests;

namespace Aero.Scanner.WebApp.Services;

public sealed class SystemConfigurationApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<SystemConfigurationDto>> GetAllAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync("/api/system-configs", ct);
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<SystemConfigurationDto>>(ct) ?? [];
    }

    public async Task<IReadOnlyList<SystemConfigurationDto>> GetByPrefixAsync(string keyPrefix, CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return [.. all.Where(x => x.Key.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase))];
    }

    public async Task<(bool Success, SystemConfigurationDto? Data, string? Error)> CreateAsync(
        CreateSystemConfigurationRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/system-configs", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<SystemConfigurationDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, SystemConfigurationDto? Data, string? Error)> UpdateAsync(
        string key, UpdateSystemConfigurationRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/system-configs/{key}", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<SystemConfigurationDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<bool> DeleteAsync(string key, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/system-configs/{key}", ct);
        return response.IsSuccessStatusCode;
    }

}