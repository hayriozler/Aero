using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Web.Services.Identity;
using Aero.Flights.Web.Services.Metadatas;

namespace Aero.Flights.Web.Services;

public sealed class ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IApiClient
{
    public async Task<LoginResult?> LoginAsync(string tenantId, string username, string password, CancellationToken ct = default)
    {
        using var response = await Http.PostAsJsonAsync(
            $"/{Uri.EscapeDataString(tenantId)}/api/auth/login", new { username, password }, JsonOptions, ct);

        if (!response.IsSuccessStatusCode)
            return null;

        var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResult>>(JsonOptions, ct);
        return envelope?.Data;
    }

    public Task<IReadOnlyList<GateResponse>> GetGatesAsync(CancellationToken ct = default)
        => GetListAsync<GateResponse>(MetadataCatalog.RouteOf("gates"), ct);

    public Task<IReadOnlyList<AirlineResponse>> GetAirlinesAsync(CancellationToken ct = default)
        => GetListAsync<AirlineResponse>(MetadataCatalog.RouteOf("airlines"), ct);

    public Task<IReadOnlyList<AircraftResponse>> GetAircraftsAsync(CancellationToken ct = default)
        => GetListAsync<AircraftResponse>(MetadataCatalog.RouteOf("aircraft"), ct);

    public Task<IReadOnlyList<AircraftTypeResponse>> GetAircraftTypesAsync(CancellationToken ct = default)
        => GetListAsync<AircraftTypeResponse>(MetadataCatalog.RouteOf("aircrafttypes"), ct);

    public Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken ct = default)
        => GetListAsync<RoleDto>("/api/roles", ct);

    public Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default)
        => GetListAsync<UserDto>("/api/users", ct);

    public Task<UserDto> CreateUserAsync(string username, string password, string? realName, CancellationToken ct = default)
        => PostForResultAsync<UserDto>("/api/users", new { username, password, realName }, ct);

    public Task<RoleDto> CreateRoleAsync(string name, string? description, CancellationToken ct = default)
        => PostForResultAsync<RoleDto>("/api/roles", new { name, description }, ct);

    public Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default)
        => GetListAsync<PermissionDto>("/api/permissions", ct);

    public Task<IReadOnlyList<string>> GetRolePermissionsAsync(int roleId, CancellationToken ct = default)
        => GetListAsync<string>($"/api/roles/{roleId}/permissions", ct);

    public Task SetRolePermissionsAsync(int roleId, IReadOnlyList<string> permissionCodes, CancellationToken ct = default)
        => SendAsync(HttpMethod.Put, $"/api/roles/{roleId}/permissions", new { permissionCodes }, ct);

    public Task AssignUserRoleAsync(int userId, int roleId, CancellationToken ct = default)
        => SendAsync(HttpMethod.Post, $"/api/users/{userId}/roles", new { roleId }, ct);

    public Task RemoveUserRoleAsync(int userId, int roleId, CancellationToken ct = default)
        => SendAsync(HttpMethod.Delete, $"/api/users/{userId}/roles/{roleId}", null, ct);

    public Task<bool> DeleteUserAsync(int userId, CancellationToken ct = default)
        => DeleteAsync($"/api/users/{userId}", ct);

    public Task<bool> DeleteRoleAsync(int roleId, CancellationToken ct = default)
        => DeleteAsync($"/api/roles/{roleId}", ct);

    public Task<IReadOnlyList<AuditEntryDto>> GetUserAuditAsync(int userId, CancellationToken ct = default)
        => GetListAsync<AuditEntryDto>($"/api/users/{userId}/audit", ct);

    public Task<IReadOnlyList<AuditEntryDto>> GetRoleAuditAsync(int roleId, CancellationToken ct = default)
        => GetListAsync<AuditEntryDto>($"/api/roles/{roleId}/audit", ct);
}
