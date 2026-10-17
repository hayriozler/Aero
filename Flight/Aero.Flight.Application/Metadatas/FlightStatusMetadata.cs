using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class FlightStatusReader : IMetadataReader<FlightStatus, FlightStatusResponse>
{
    public string GetCode(FlightStatus entity) => entity.Code;

    public FlightStatusResponse ToResponse(FlightStatus entity) => new(
        entity.Code, entity.Description, entity.IsUpdatable);

    public Expression<Func<FlightStatus, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<FlightStatus, FlightStatusResponse>> Projection { get; } =
        e => new FlightStatusResponse(e.Code, e.Description, e.IsUpdatable);
}
