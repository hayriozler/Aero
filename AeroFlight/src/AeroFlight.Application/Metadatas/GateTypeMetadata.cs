using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class GateTypeReader : IMetadataReader<GateType, GateTypeResponse>
{
    public string GetCode(GateType entity) => entity.Code;
    public GateTypeResponse ToResponse(GateType entity) => new(entity.Code, entity.Description);

    public Expression<Func<GateType, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<GateType, GateTypeResponse>> Projection { get; } =
        e => new GateTypeResponse(e.Code, e.Description);
}
