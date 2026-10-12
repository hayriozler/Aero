using AeroFlight.Contracts.Identity;
using AeroFlight.Domain.Identity;

namespace AeroFlight.Application.Identity.Roles;

internal static class RoleResponseFactory
{
    public static RoleResponse From(Role role) => new(
        role.Id, role.Name, role.Description,
        role.CreatedAt, role.CreatedBy, role.UpdatedAt, role.UpdatedBy);
}
