using Aero.Scanner.Shared.Dtos;
using Aero.Scanner.Shared.Requests;

namespace Aero.Scanner.WebApp.Services;

public sealed class RoleApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync("/api/roles", ct);
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<RoleDto>>(ct) ?? [];
    }

    public async Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync("/api/permissions", ct);
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<PermissionDto>>(ct) ?? [];
    }

    public async Task<(bool Success, RoleDto? Data, string? Error)> CreateAsync(
        CreateRoleRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/roles", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<RoleDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, RoleDto? Data, string? Error)> UpdateAsync(
        string roleName, UpdateRoleRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/roles/{roleName}", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<RoleDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<bool> DeleteAsync(string roleName, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/roles/{roleName}", ct);
        return response.IsSuccessStatusCode;
    }
}
