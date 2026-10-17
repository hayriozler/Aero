using Aero.Flights.Domain.Common;
namespace Aero.Flights.Domain.Metadatas;

public sealed class CompanyType : AuditBase<string>
{
    public string Description { get; private set; } = string.Empty;

    private CompanyType() { }

    private CompanyType(string code, string description)
    {
        SetId(code);
        Description = description;
    }

    public static CompanyType Create(string code, string description) => new(code, description);

    public void Update(string description) => Description = description;
}
