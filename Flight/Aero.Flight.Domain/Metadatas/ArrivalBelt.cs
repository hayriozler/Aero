using Aero.Flights.Domain.Common;
namespace Aero.Flights.Domain.Metadatas;

public sealed class ArrivalBelt : AuditBase<string>
{
    public string? Description { get; private set; }
    public int? Capacity { get; private set; }

    public string? TerminalCode { get; private set; }

    private ArrivalBelt() { }

    private ArrivalBelt(string code, string? terminalCode)
    {
        SetId(code);
        TerminalCode = terminalCode;
    }

    public static ArrivalBelt Create(string code, string? terminalCode) => new(code, terminalCode);

    public void Update(string? terminalCode, string? description, int? capacity)
    {
        TerminalCode = terminalCode;
        Description = description;
        Capacity = capacity;
    }
}
