using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class CounterStatusReader : IMetadataReader<CounterStatus, CounterStatusResponse>
{
    public string GetCode(CounterStatus entity) => entity.Code;
    public CounterStatusResponse ToResponse(CounterStatus entity) => new(entity.Code, entity.Description, entity.IsDefault, entity.IsOpenValue, entity.IsCloseValue);

    public Expression<Func<CounterStatus, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<CounterStatus, CounterStatusResponse>> Projection { get; } =
        e => new CounterStatusResponse(e.Code, e.Description, e.IsDefault, e.IsOpenValue, e.IsCloseValue);
}
