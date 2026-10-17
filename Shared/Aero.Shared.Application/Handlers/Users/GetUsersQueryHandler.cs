using Aero.Shared.Application.Queries.Users;
using Aero.Shared.Contracts.Responses;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Shared.Application.Handlers.Users;

public sealed class GetUsersQueryHandler(IDataStoreContext db) : IQueryHandler<GetUsersQuery, IReadOnlyList<UserResponse>>
{
    public async Task<IReadOnlyList<UserResponse>> HandleAsync(GetUsersQuery query, CancellationToken ct)
    {
        var users = await db.UserSet
            .Include(u => u.Roles).ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Username)
            .ToListAsync(ct);


        return users.Select(user => new UserResponse(
            user.Id,
            user.Username,
            user.RealName,
            user.IsActive,
            user.ExpirationDate,
            user.Roles
                .Where(ur => ur.Role is not null)
                .Select(ur => ur.Role!.Id)
                .ToList(),
            user.CreatedAt,
            user.CreatedBy,
            user.UpdatedAt,
            user.UpdatedBy
        )).ToList();
    }
}
