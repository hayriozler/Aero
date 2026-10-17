using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class AircraftNoiseCategoryReader : IMetadataReader<AircraftNoiseCategory, AircraftNoiseCategoryResponse>
{
    public string GetCode(AircraftNoiseCategory entity) => entity.Code;
    public AircraftNoiseCategoryResponse ToResponse(AircraftNoiseCategory entity) => new(entity.Code, entity.Description);

    public Expression<Func<AircraftNoiseCategory, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<AircraftNoiseCategory, AircraftNoiseCategoryResponse>> Projection { get; } =
        e => new AircraftNoiseCategoryResponse(e.Code, e.Description);
}
