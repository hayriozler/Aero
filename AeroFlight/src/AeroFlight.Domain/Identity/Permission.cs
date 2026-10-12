namespace AeroFlight.Domain.Identity;

public sealed class Permission
{
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private Permission() { }

    private Permission(string code, string? description)
    {
        Code = code;
        Description = description;
    }

    public static Permission Create(string code, string? description = null) => new(code, description);
}
