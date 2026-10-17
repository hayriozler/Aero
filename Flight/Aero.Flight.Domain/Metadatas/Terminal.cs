using Aero.Flights.Domain.Common;

namespace Aero.Flights.Domain.Metadatas;

public sealed class Terminal : AuditBase<string>
{
    public string? Description { get; private set; }
    private Terminal() { }

    private Terminal(string code) => SetId(code);

    public static Terminal Create(string code) => new(code);

    public void Update(string? description) => Description = description;
}
