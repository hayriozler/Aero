using Aero.Scanner.Domain.Enums;
using Aero.Shared.Domain;

namespace Aero.Scanner.Domain.Aggregates.Companies;

public sealed class Company : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;

    public CompanyType Type { get; private set; }
    // EF Core
    private Company() { }

    public static Company Create(string code, string name, CompanyType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Company
        {
            Code = code.ToUpperInvariant().Trim(),
            Name = name.Trim(),
            Type = type
        };
    }

    public void Update(string code, string name, CompanyType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Code = code.ToUpperInvariant().Trim();
        Name = name.Trim();
        Type = type;
    }
}
