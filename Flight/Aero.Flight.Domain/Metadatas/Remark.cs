using Aero.Flights.Domain.Common;
namespace Aero.Flights.Domain.Metadatas;

public sealed class Remark : AuditBase<string>
{
    public string Description { get; private set; } = string.Empty;

    private Remark() { }

    private Remark(string code, string description)
    {
        SetId(code);
        Description  =description;
    }

    public static Remark Create(string code, string description) => new(code, description);

    public void Update(string description) => Description = description;
}
