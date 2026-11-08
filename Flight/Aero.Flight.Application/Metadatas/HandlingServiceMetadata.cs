using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;


public sealed class HandlingServiceMetadataMap : IMetadataMap<HandlingService, HandlingServiceRequest, HandlingServiceResponse>
{
    public string GetCode(HandlingService entity) => entity.Id;

    public HandlingService Create(HandlingServiceRequest r) => HandlingService.Create(r.Code);

    public void Apply(HandlingService e, HandlingServiceRequest r) => e.Update(r.Unit, r.Description);

    public HandlingServiceResponse ToResponse(HandlingService e) => new(
        e.Id, e.Unit, e.Description);

    public Expression<Func<HandlingService, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<HandlingService, HandlingServiceResponse>> Projection { get; } = e => new HandlingServiceResponse(
        e.Id, e.Unit, e.Description);
}
