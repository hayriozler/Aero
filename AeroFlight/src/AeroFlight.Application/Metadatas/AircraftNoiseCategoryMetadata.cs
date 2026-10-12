using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class AircraftNoiseCategoryReader : IMetadataReader<AircraftNoiseCategory, AircraftNoiseCategoryResponse>
{
    public string GetCode(AircraftNoiseCategory entity) => entity.Code;
    public AircraftNoiseCategoryResponse ToResponse(AircraftNoiseCategory entity) => new(entity.Code, entity.Description);

    public Expression<Func<AircraftNoiseCategory, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<AircraftNoiseCategory, AircraftNoiseCategoryResponse>> Projection { get; } =
        e => new AircraftNoiseCategoryResponse(e.Code, e.Description);
}
