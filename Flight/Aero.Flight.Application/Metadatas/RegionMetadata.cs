using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class RegionReader : IMetadataReader<Region, RegionResponse>
{
    public string GetCode(Region entity) => entity.Code;
    public RegionResponse ToResponse(Region entity) => new(entity.Code, entity.Description);

    public Expression<Func<Region, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<Region, RegionResponse>> Projection { get; } =
        e => new RegionResponse(e.Code, e.Description);
}
