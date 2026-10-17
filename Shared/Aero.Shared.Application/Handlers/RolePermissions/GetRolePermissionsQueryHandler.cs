using Aero.Shared.Application.Queries.RolePermissions;
using Aero.Shared.Domain.Identity;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.RolePermissions;

public sealed class GetRolePermissionsQueryHandler(IDataStoreContext db)
    : IQueryHandler<GetRolePermissionsQuery, IReadOnlyList<string>>
{
    public async Task<IReadOnlyList<string>> HandleAsync(GetRolePermissionsQuery query, CancellationToken ct)
        => await db.Query<RolePermission>()
            .Where(rp => rp.RoleName == query.RoleName)
            .Select(rp => rp.PermissionCode)
            .ToListAsync(ct);
}

