using AeroFlight.Contracts.Identity;
using AeroFlight.Domain.Identity;

namespace AeroFlight.Application.Identity.Users;

internal static class UserResponseFactory
{
    public static UserResponse From(User user, IReadOnlyList<string> roles) => new(
        user.Id, user.Username, user.RealName, user.IsActive, user.ExpirationDate,
        roles, user.CreatedAt, user.CreatedBy, user.UpdatedAt, user.UpdatedBy);
}
