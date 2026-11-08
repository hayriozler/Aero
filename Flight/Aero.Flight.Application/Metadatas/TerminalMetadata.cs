using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;



public sealed class TerminalMetadataMap : IMetadataMap<Terminal, TerminalRequest, TerminalResponse>
{
    public string GetCode(Terminal entity) => entity.Id;

    public Terminal Create(TerminalRequest r) => Terminal.Create(r.Code);

    public void Apply(Terminal e, TerminalRequest r) => e.Update(r.Description);

    public TerminalResponse ToResponse(Terminal e) => new(
        e.Id, e.Description);

    public Expression<Func<Terminal, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<Terminal, TerminalResponse>> Projection { get; } = e => new TerminalResponse(
        e.Id, e.Description);
}
