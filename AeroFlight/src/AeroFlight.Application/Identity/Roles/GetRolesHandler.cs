using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Identity;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Roles;

public record GetRolesQuery;

public sealed class GetRolesQueryHandler(IAeroFlightDbContext db) : IQueryHandler<GetRolesQuery, IReadOnlyList<RoleResponse>>
{
    public async Task<IReadOnlyList<RoleResponse>> HandleAsync(GetRolesQuery query, CancellationToken ct)
    {
        var roles = await db.RoleSet.OrderBy(r => r.Name).ToListAsync(ct);
        return roles.Select(RoleResponseFactory.From).ToList();
    }
}
