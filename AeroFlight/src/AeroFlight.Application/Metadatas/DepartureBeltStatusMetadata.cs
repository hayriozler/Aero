using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class DepartureBeltStatusReader : IMetadataReader<DepartureBeltStatus, DepartureBeltStatusResponse>
{
    public string GetCode(DepartureBeltStatus entity) => entity.Code;
    public DepartureBeltStatusResponse ToResponse(DepartureBeltStatus entity) => new(entity.Code, entity.Description, entity.IsDefault, entity.IsOpenValue, entity.IsCloseValue);

    public Expression<Func<DepartureBeltStatus, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<DepartureBeltStatus, DepartureBeltStatusResponse>> Projection { get; } =
        e => new DepartureBeltStatusResponse(e.Code, e.Description, e.IsDefault, e.IsOpenValue, e.IsCloseValue);
}
