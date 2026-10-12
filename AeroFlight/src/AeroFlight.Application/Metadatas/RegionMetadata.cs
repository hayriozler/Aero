using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class RegionReader : IMetadataReader<Region, RegionResponse>
{
    public string GetCode(Region entity) => entity.Code;
    public RegionResponse ToResponse(Region entity) => new(entity.Code, entity.Description);

    public Expression<Func<Region, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<Region, RegionResponse>> Projection { get; } =
        e => new RegionResponse(e.Code, e.Description);
}
