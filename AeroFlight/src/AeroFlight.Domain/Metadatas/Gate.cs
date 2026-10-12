using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Metadatas;

public sealed class Gate : AuditBase<string>
{
    public string? GateTypeCode { get; private set; }
    public string? Description { get; private set; }
    public string? TerminalCode { get; private set; }
    public int? Capacity { get; private set; }
    public bool HasLoadingBridge { get; private set; }
    public bool HasPca { get; private set; }
    private Gate() { }

    private Gate(string code, string? terminalCode, string? gateTypeCode)
    {
        SetId(code);
        TerminalCode = terminalCode;
        GateTypeCode = gateTypeCode;
    }

    public static Gate Create(string code, string? terminalCode, string? gateTypeCode = null)
        => new(code, terminalCode, gateTypeCode);

    public void Update(string? gateTypeCode, string? terminalCode, string? description, int? capacity, bool hasLoadingBridge, bool hasPca)
    {
        GateTypeCode = gateTypeCode;
        Description = description;
        Capacity = capacity;
        HasLoadingBridge = hasLoadingBridge;
        HasPca = hasPca;
        TerminalCode = terminalCode;
    }
}
