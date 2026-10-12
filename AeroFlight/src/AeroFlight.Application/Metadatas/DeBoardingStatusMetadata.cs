using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class DeBoardingStatusReader : IMetadataReader<DeBoardingStatus, DeBoardingStatusResponse>
{
    public string GetCode(DeBoardingStatus entity) => entity.Code;
    public DeBoardingStatusResponse ToResponse(DeBoardingStatus entity) => new(entity.Code, entity.Description);

    public Expression<Func<DeBoardingStatus, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<DeBoardingStatus, DeBoardingStatusResponse>> Projection { get; } =
        e => new DeBoardingStatusResponse(e.Code, e.Description);
}
