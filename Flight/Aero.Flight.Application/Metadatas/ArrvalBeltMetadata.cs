using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;



public sealed class ArrivalBeltMetadataMap
    : IMetadataMap<ArrivalBelt, ArrivalBeltRequest, ArrivalBeltResponse>
{
    public string GetCode(ArrivalBelt entity) => entity.Id;

    public ArrivalBelt Create(ArrivalBeltRequest r) =>
        ArrivalBelt.Create(r.Code, r.TerminalCode);

    public void Apply(ArrivalBelt e, ArrivalBeltRequest r) => e.Update(r.TerminalCode, r.Description, r.Capacity);

    public ArrivalBeltResponse ToResponse(ArrivalBelt e) => new(e.Id, e.TerminalCode, e.Description, e.Capacity);

    public Expression<Func<ArrivalBelt, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<ArrivalBelt, ArrivalBeltResponse>> Projection { get; } =
        e => new ArrivalBeltResponse(e.Id, e.TerminalCode, e.Description, e.Capacity);
}
