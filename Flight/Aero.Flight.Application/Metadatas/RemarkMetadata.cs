using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;



public sealed class RemarkMetadataMap : IMetadataMap<Remark, RemarkRequest, RemarkResponse>
{
    public string GetCode(Remark entity) => entity.Id;

    public Remark Create(RemarkRequest r) => Remark.Create(r.Code, r.Description);

    public void Apply(Remark e, RemarkRequest r) => e.Update(r.Description);

    public RemarkResponse ToResponse(Remark e) => new(
        e.Id, e.Description);

    public Expression<Func<Remark, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<Remark, RemarkResponse>> Projection { get; } = e => new RemarkResponse(
        e.Id, e.Description);
}
