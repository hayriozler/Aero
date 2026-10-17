using Aero.Shared.Application.Queries.RolePermissions;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.Roles;



public sealed class GetRolesQueryHandler(IDataStoreContext db) : IQueryHandler<GetRolesQuery, IReadOnlyList<RoleResponse>>
{
    public async Task<IReadOnlyList<RoleResponse>> HandleAsync(GetRolesQuery query, CancellationToken ct)
    {
        var roles = await db.RoleSet.OrderBy(r => r.Id).ToListAsync(ct);
        return roles.Select(r => new RoleResponse(
            r.Id, r.Description,
            r.CreatedAt, r.CreatedBy, r.UpdatedAt, r.UpdatedBy)).ToList();
    }
}
