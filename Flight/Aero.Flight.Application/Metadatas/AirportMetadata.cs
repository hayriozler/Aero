using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;



public sealed class AirportMetadataMap : IMetadataMap<Airport, AirportRequest, AirportResponse>
{
    public string GetCode(Airport entity) => entity.Id;

    public Airport Create(AirportRequest r) => Airport.Create(r.Code, r.IataCode, r.Name, r.DisplayName);

    public void Apply(Airport e, AirportRequest r) => e.Update(
        r.IataCode, r.IcaoCode, r.Name, r.DisplayName, r.CountryCode, r.TimezoneCode);

    public AirportResponse ToResponse(Airport e) => new(
        e.Id, e.IataCode, e.IcaoCode, e.Name, e.DisplayName, e.CountryCode, e.TimezoneCode);

    public Expression<Func<Airport, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<Airport, AirportResponse>> Projection { get; } = e => new AirportResponse(
        e.Id, e.IataCode, e.IcaoCode, e.Name, e.DisplayName, e.CountryCode, e.TimezoneCode);
}
