using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas;

public sealed class CounterRequestValidator : AbstractValidator<CounterRequest>
{
    public CounterRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}

public sealed class CounterMetadataMap : IMetadataMap<Counter, CounterRequest, CounterResponse>
{
    public string GetCode(Counter entity) => entity.Id;

    public Counter Create(CounterRequest r) => Counter.Create(r.Code, r.TerminalCode);

    public void Apply(Counter e, CounterRequest r) => e.Update(
        r.TerminalCode, r.Description, r.Capacity);

    public CounterResponse ToResponse(Counter e) => new(
        e.Id, e.TerminalCode, e.Description, e.Capacity);

    public System.Linq.Expressions.Expression<Func<Counter, string>> CodeSelector { get; } = e => e.Id;

    public System.Linq.Expressions.Expression<Func<Counter, CounterResponse>> Projection { get; } = e => new CounterResponse(
        e.Id, e.TerminalCode, e.Description, e.Capacity);
}
