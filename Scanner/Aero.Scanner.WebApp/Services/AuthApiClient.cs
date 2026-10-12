using Aero.Scanner.Shared.Dtos;
using Aero.Scanner.Shared.Requests;

namespace Aero.Scanner.WebApp.Services;

public sealed class AuthApiClient(HttpClient client)
{
    public async Task<LoginResponseDto?> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(username, password), ct);

        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<LoginResponseDto>(ct);
    }

    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync("/api/auth/change-password",
            new ChangePasswordRequest(currentPassword, newPassword), ct);
        return response.IsSuccessStatusCode;
    }
}
