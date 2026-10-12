using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class ArrivalBeltStatusReader : IMetadataReader<ArrivalBeltStatus, ArrivalBeltStatusResponse>
{
    public string GetCode(ArrivalBeltStatus entity) => entity.Code;
    public ArrivalBeltStatusResponse ToResponse(ArrivalBeltStatus entity) => new(entity.Code, entity.Description, entity.IsDefault, entity.IsOpenValue, entity.IsCloseValue);

    public Expression<Func<ArrivalBeltStatus, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<ArrivalBeltStatus, ArrivalBeltStatusResponse>> Projection { get; } =
        e => new ArrivalBeltStatusResponse(e.Code, e.Description, e.IsDefault, e.IsOpenValue, e.IsCloseValue);
}
