using Aero.Flights.Application.Common;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.Roles;



public sealed class GetRolesQueryHandler(IDataStoreContext db) : IQueryHandler<GetRolesQuery, IReadOnlyList<RoleResponse>>
{
    public async Task<IReadOnlyList<RoleResponse>> HandleAsync(GetRolesQuery query, CancellationToken ct)
    {
        var roles = await db.RoleSet.OrderBy(r => r.Name).ToListAsync(ct);
        return roles.Select(RoleResponseFactory.From).ToList();
    }
}
