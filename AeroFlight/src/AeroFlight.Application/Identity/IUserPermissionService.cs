namespace AeroFlight.Application.Identity;

public interface IUserPermissionService
{
    Task<IReadOnlySet<string>> GetPermissionsAsync(IReadOnlyCollection<string> roleNames, CancellationToken ct);
}
