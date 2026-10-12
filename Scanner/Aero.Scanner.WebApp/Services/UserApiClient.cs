using Aero.Scanner.Shared.Dtos;
using Aero.Scanner.Shared.Requests;

namespace Aero.Scanner.WebApp.Services;

public sealed class UserApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync("/api/users", ct);
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<UserDto>>(ct) ?? [];
    }

    public async Task<(bool Success, UserDto? Data, string? Error)> CreateAsync(
        CreateUserRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("/api/users", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<UserDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, UserDto? Data, string? Error)> UpdateAsync(
        int id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"/api/users/{id}", request, ct);
        if (response.IsSuccessStatusCode)
            return (true, await response.Content.ReadFromJsonAsync<UserDto>(ct), null);
        return (false, null, await response.ReadApiErrorAsync(ct));
    }

    public async Task<(bool Success, string? Error)> DeactivateAsync(int id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"/api/users/{id}", ct);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await response.ReadApiErrorAsync(ct));
    }

    public async Task<bool> ResetPasswordAsync(int id, AdminResetPasswordRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync($"/api/users/{id}/reset-password", request, ct);
        return response.IsSuccessStatusCode;
    }
}
