using Aero.Flights.Application.Common;
using Aero.Shared.Application.Queries.Roles;
using Aero.Shared.Contracts.Response;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.Roles;

public sealed class GetRoleQueryHandler(IDataStoreContext db) : IQueryHandler<GetRoleQuery, RoleResponse?>
{
    public async Task<RoleResponse?> HandleAsync(GetRoleQuery query, CancellationToken ct)
    {
        var role = await db.RoleSet.FirstOrDefaultAsync(r => r.Id == query.Id, ct);
        return role is null ? null : RoleResponseFactory.From(role);
    }
}
