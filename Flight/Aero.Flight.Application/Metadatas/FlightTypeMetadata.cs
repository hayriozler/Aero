using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class FlightTypeReader : IMetadataReader<FlightType, FlightTypeResponse>
{
    public string GetCode(FlightType entity) => entity.Code;

    public FlightTypeResponse ToResponse(FlightType entity) => new(entity.Code, entity.Description, entity.DisplayCode);

    public Expression<Func<FlightType, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<FlightType, FlightTypeResponse>> Projection { get; } =
        e => new FlightTypeResponse(e.Code, e.Description, e.DisplayCode);
}
