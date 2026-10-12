using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Metadatas;

public sealed class CheckinClass : AuditBase<string>
{
    public string Description { get; private set; } = string.Empty;

    private CheckinClass() { }

    private CheckinClass(string code, string description)
    {
        SetId(code);
        Description = description;
    }

    public static CheckinClass Create(string code, string description) => new(code, description);

    public void Update(string description) => Description = description;
}
