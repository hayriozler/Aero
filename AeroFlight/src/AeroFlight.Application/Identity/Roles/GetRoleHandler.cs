using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Identity;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Roles;

public record GetRoleQuery(int Id);

public sealed class GetRoleQueryHandler(IAeroFlightDbContext db) : IQueryHandler<GetRoleQuery, RoleResponse?>
{
    public async Task<RoleResponse?> HandleAsync(GetRoleQuery query, CancellationToken ct)
    {
        var role = await db.RoleSet.FirstOrDefaultAsync(r => r.Id == query.Id, ct);
        return role is null ? null : RoleResponseFactory.From(role);
    }
}
