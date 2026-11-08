using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;



public sealed class AircraftSizeCategoryMetadataMap
    : IMetadataMap<AircraftSizeCategory, AircraftSizeCategoryRequest, AircraftSizeCategoryResponse>
{
    public string GetCode(AircraftSizeCategory entity) => entity.Id;

    public AircraftSizeCategory Create(AircraftSizeCategoryRequest r) =>
        AircraftSizeCategory.Create(r.Code, r.Description);

    public void Apply(AircraftSizeCategory e, AircraftSizeCategoryRequest r) => e.Update(r.Description);

    public AircraftSizeCategoryResponse ToResponse(AircraftSizeCategory e) => new(
        e.Id, e.Description);

    public Expression<Func<AircraftSizeCategory, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<AircraftSizeCategory, AircraftSizeCategoryResponse>> Projection { get; } =
        e => new AircraftSizeCategoryResponse(e.Id, e.Description);
}
