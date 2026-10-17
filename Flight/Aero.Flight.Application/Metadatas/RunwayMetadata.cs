using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class RunwayRequestValidator : AbstractValidator<RunwayRequest>
{
    public RunwayRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Name).MaximumLength(100);
    }
}

public sealed class RunwayMetadataMap : IMetadataMap<Runway, RunwayRequest, RunwayResponse>
{
    public string GetCode(Runway entity) => entity.Id;

    public Runway Create(RunwayRequest r) => Runway.Create(r.Code, r.Name);

    public void Apply(Runway e, RunwayRequest r) => e.Update(r.Name, r.Length, r.Width);

    public RunwayResponse ToResponse(Runway e) => new(
        e.Id, e.Name, e.Length, e.Width);

    public Expression<Func<Runway, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<Runway, RunwayResponse>> Projection { get; } = e => new RunwayResponse(
        e.Id, e.Name, e.Length, e.Width);
}
