using Aero.Shared.Application.Queries.Roles;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.Roles;

public sealed class GetRoleQueryHandler(IDataStoreContext db) : IQueryHandler<GetRoleQuery, RoleResponse?>
{
    public async Task<RoleResponse?> HandleAsync(GetRoleQuery query, CancellationToken ct)
    {
        var role = await db.RoleSet.FirstOrDefaultAsync(r => r.Id == query.Name, ct);
        return role is null ? null : new(
        role.Id, role.Description,
        role.CreatedAt, role.CreatedBy, role.UpdatedAt, role.UpdatedBy);
    }
}
