namespace Aero.Flights.Domain.Metadatas;

public sealed class DepartureBeltStatus : IResourceStatus
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }
    public bool IsOpenValue { get; private set; }
    public bool IsCloseValue { get; private set; }

    private DepartureBeltStatus() { }
    public DepartureBeltStatus(string code, string description)
    {
        Code = code;
        Description = description;
    }
    public static DepartureBeltStatus Create(string code, string description)
           => new(code, description);
}
