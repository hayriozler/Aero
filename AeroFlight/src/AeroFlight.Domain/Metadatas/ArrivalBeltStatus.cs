namespace AeroFlight.Domain.Metadatas;

public sealed class ArrivalBeltStatus : IResourceStatus
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }
    public bool IsOpenValue { get; private set; }
    public bool IsCloseValue { get; private set; }

    private ArrivalBeltStatus() { }
    public ArrivalBeltStatus(string code, string description)
    {
        Code = code;
        Description = description;
    }
    public static ArrivalBeltStatus Create(string code, string description)
           => new(code, description);
}
