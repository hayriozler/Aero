using Aero.Flights.Domain.Common;

namespace Aero.Shared.Domain.Identity;

public sealed class Role : AuditBase<string>
{
    private readonly List<UserRole> _userRoles = [];

    public string? Description { get; private set; }

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    private Role() { }

    private Role(string name, string? description)
    {
        SetId(name);
        Description = description;
    }

    public static Role Create(string name, string? description = null) => new(name, description);

    public void Update(string name, string? description)
    {
        SetId(name);
        Description = description;
    }
}
