using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Metadatas;

public sealed class Counter : AuditBase<string>
{
    public string? Description { get; private set; }
    public int? Capacity { get; private set; }
    public string? TerminalCode { get; private set; }
    private Counter() { }

    private Counter(string code, string? terminalCode)
    {
        SetId(code);
        TerminalCode = terminalCode;
    }

    public static Counter Create(string code, string? terminalCode)
        => new(code, terminalCode);

    public void Update(
        string? terminalCode,
        string? description,
        int? capacity)
    {
        TerminalCode =terminalCode;
        Description = description;
        Capacity = capacity;
    }
}
