using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Identity;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Users;

public record GetUsersQuery;

public sealed class GetUsersQueryHandler(IAeroFlightDbContext db) : IQueryHandler<GetUsersQuery, IReadOnlyList<UserResponse>>
{
    public async Task<IReadOnlyList<UserResponse>> HandleAsync(GetUsersQuery query, CancellationToken ct)
    {
        var users = await db.UserSet
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Username)
            .ToListAsync(ct);

        return users
            .Select(u => UserResponseFactory.From(u, u.UserRoles.Where(ur => ur.Role is not null).Select(ur => ur.Role!.Name).ToList()))
            .ToList();
    }
}
