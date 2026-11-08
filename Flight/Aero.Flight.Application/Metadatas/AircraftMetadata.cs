using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;


public sealed class AircraftMetadataMap : IMetadataMap<Aircraft, AircraftRequest, AircraftResponse>
{
    public string GetCode(Aircraft entity) => entity.Id;

    public Aircraft Create(AircraftRequest r) => Aircraft.Create(r.Code, r.AircraftTypeCode);

    public void Apply(Aircraft e, AircraftRequest r) => e.Update(
        r.AirlineCode, r.Owner, r.SizeCategory, r.NoiseCategory, r.BillWeight, r.SeatCapacity);

    public AircraftResponse ToResponse(Aircraft e) => new(
        e.Id, e.AircraftTypeCode, e.AirlineCode, e.Owner, e.SizeCategory, e.NoiseCategory, e.Weight, e.SeatCapacity);

    public Expression<Func<Aircraft, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<Aircraft, AircraftResponse>> Projection { get; } = e => new AircraftResponse(
        e.Id, e.AircraftTypeCode, e.AirlineCode, e.Owner, e.SizeCategory, e.NoiseCategory, e.Weight, e.SeatCapacity);
}
