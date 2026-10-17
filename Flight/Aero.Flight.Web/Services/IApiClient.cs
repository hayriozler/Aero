using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Web.Services.Identity;
using Aero.Flights.Web.Services.Metadatas;

namespace Aero.Flights.Web.Services;

public interface IApiClient
{
    Task<LoginResult?> LoginAsync(string tenantId, string username, string password, CancellationToken ct = default);

    Task<IReadOnlyList<GateResponse>> GetGatesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<AirlineResponse>> GetAirlinesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<AircraftResponse>> GetAircraftsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<AircraftTypeResponse>> GetAircraftTypesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default);
    Task<UserDto> CreateUserAsync(string username, string password, string? realName, CancellationToken ct = default);
    Task<RoleDto> CreateRoleAsync(string name, string? description, CancellationToken ct = default);
    Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetRolePermissionsAsync(int roleId, CancellationToken ct = default);
    Task SetRolePermissionsAsync(int roleId, IReadOnlyList<string> permissionCodes, CancellationToken ct = default);
    Task AssignUserRoleAsync(int userId, int roleId, CancellationToken ct = default);
    Task RemoveUserRoleAsync(int userId, int roleId, CancellationToken ct = default);
    Task<bool> DeleteUserAsync(int userId, CancellationToken ct = default);
    Task<bool> DeleteRoleAsync(int roleId, CancellationToken ct = default);
    Task<IReadOnlyList<AuditEntryDto>> GetUserAuditAsync(int userId, CancellationToken ct = default);
    Task<IReadOnlyList<AuditEntryDto>> GetRoleAuditAsync(int roleId, CancellationToken ct = default);
}
