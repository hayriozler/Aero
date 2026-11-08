using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;



public sealed class DepartureBeltMetadataMap
    : IMetadataMap<DepartureBelt, DepartureBeltRequest, DepartureBeltResponse>
{
    public string GetCode(DepartureBelt entity) => entity.Id;

    public DepartureBelt Create(DepartureBeltRequest r) =>
        DepartureBelt.Create(r.Code, r.TerminalCode);

    public void Apply(DepartureBelt e, DepartureBeltRequest r) => e.Update(r.TerminalCode, r.Description, r.Capacity);

    public DepartureBeltResponse ToResponse(DepartureBelt e) => new(e.Id, e.TerminalCode, e.Description, e.Capacity);

    public Expression<Func<DepartureBelt, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<DepartureBelt, DepartureBeltResponse>> Projection { get; } =
        e => new DepartureBeltResponse(e.Id, e.TerminalCode, e.Description, e.Capacity);
}
