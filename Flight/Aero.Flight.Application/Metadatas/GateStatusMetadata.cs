using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class GateStatusReader : IMetadataReader<GateStatus, GateStatusResponse>
{
    public string GetCode(GateStatus entity) => entity.Code;
    public GateStatusResponse ToResponse(GateStatus entity) => new(entity.Code, entity.Description, entity.IsDefault, entity.IsOpenValue, entity.IsCloseValue);

    public Expression<Func<GateStatus, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<GateStatus, GateStatusResponse>> Projection { get; } =
        e => new GateStatusResponse(e.Code, e.Description, e.IsDefault, e.IsOpenValue, e.IsCloseValue);
}
