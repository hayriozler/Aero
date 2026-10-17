using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class AircraftRequestValidator : AbstractValidator<AircraftRequest>
{
    public AircraftRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.AircraftTypeCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.AirlineCode).MaximumLength(3);
        RuleFor(x => x.Owner).MaximumLength(100);
        RuleFor(x => x.SizeCategory).MaximumLength(5);
        RuleFor(x => x.NoiseCategory).MaximumLength(10);
    }
}
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
