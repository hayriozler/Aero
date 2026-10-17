using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class ParkPositionTypeReader : IMetadataReader<ParkPositionType, ParkPositionTypeResponse>
{
    public string GetCode(ParkPositionType entity) => entity.Code;
    public ParkPositionTypeResponse ToResponse(ParkPositionType entity) => new(entity.Code, entity.Description);

    public Expression<Func<ParkPositionType, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<ParkPositionType, ParkPositionTypeResponse>> Projection { get; } =
        e => new ParkPositionTypeResponse(e.Code, e.Description);
}
