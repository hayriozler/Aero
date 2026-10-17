namespace Aero.Shared.Infrastructure.Services;

public interface IUserPermissionService
{
    Task<IReadOnlySet<string>> GetPermissionsAsync(IReadOnlyCollection<string> roleNames, CancellationToken ct);
}
