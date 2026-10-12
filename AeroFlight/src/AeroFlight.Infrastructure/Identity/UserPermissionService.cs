using AeroFlight.Application.Identity;
using AeroFlight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Infrastructure.Identity;

internal sealed class UserPermissionService(AeroFlightDbContext db) : IUserPermissionService
{
    public async Task<IReadOnlySet<string>> GetPermissionsAsync(IReadOnlyCollection<string> roleNames, CancellationToken ct)
    {
        if (roleNames.Count == 0)
            return new HashSet<string>();

        var codes = await (from rp in db.RolePermissionSet
                           join r in db.RoleSet on rp.RoleId equals r.Id
                           where roleNames.Contains(r.Name)
                           select rp.PermissionCode).Distinct().ToListAsync(ct);

        return codes.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
