using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Infrastructure.Services;

internal sealed class UserPermissionService(IDataStoreContext db) : IUserPermissionService
{
    public async Task<IReadOnlySet<string>> GetPermissionsAsync(IReadOnlyCollection<string> roleNames, CancellationToken ct)
    {
        if (roleNames.Count == 0)
            return new HashSet<string>();
        var codes = await (from rp in db.RolePermissionSet
                           join r in db.RoleSet on rp.RoleId equals r.Id
                           where roleNames.Contains(r.Name)
                           select rp.PermissionCode).Distinct()
                           .ToListAsync(ct);

        return codes.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
