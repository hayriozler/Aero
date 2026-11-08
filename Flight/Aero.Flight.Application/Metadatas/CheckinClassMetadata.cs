using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;



public sealed class CheckinClassMetadataMap : IMetadataMap<CheckinClass, CheckinClassRequest, CheckinClassResponse>
{
    public string GetCode(CheckinClass entity) => entity.Id;

    public CheckinClass Create(CheckinClassRequest r) => CheckinClass.Create(r.Code, r.Description);

    public void Apply(CheckinClass e, CheckinClassRequest r) => e.Update(r.Description);

    public CheckinClassResponse ToResponse(CheckinClass e) => new(e.Id, e.Description);

    public Expression<Func<CheckinClass, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<CheckinClass, CheckinClassResponse>> Projection { get; } =
        e => new CheckinClassResponse(e.Id, e.Description);
}
