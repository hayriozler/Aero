using Aero.Flights.Domain.Common;
namespace Aero.Flights.Domain.Metadatas;

public sealed class DelayReason : AuditBase<string>
{
    public string? AlphabeticCode { get; private set; }
    public string? Description { get; private set; }

    private DelayReason() { }

    private DelayReason(string code, string? alphabeticCode, string? description)
    {
        SetId(code);
        AlphabeticCode = alphabeticCode;
        Description = description;
    }

    public static DelayReason Create(string code, string? alphabeticCode = null, string? description = null)
        => new(code, alphabeticCode, description);

    public void Update(string? alphabeticCode, string? description)
    {
        AlphabeticCode = alphabeticCode;
        Description = description;
    }
}
