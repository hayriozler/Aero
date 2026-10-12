using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Identity;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Identity.Users;

public record GetUserQuery(int Id);

public sealed class GetUserQueryHandler(IAeroFlightDbContext db) : IQueryHandler<GetUserQuery, UserResponse?>
{
    public async Task<UserResponse?> HandleAsync(GetUserQuery query, CancellationToken ct)
    {
        var user = await db.UserSet
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == query.Id, ct);

        if (user is null) return null;

        var roles = user.UserRoles.Where(ur => ur.Role is not null).Select(ur => ur.Role!.Name).ToList();
        return UserResponseFactory.From(user, roles);
    }
}
