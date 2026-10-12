using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;


public sealed class AircraftTypeRequestValidator : AbstractValidator<AircraftTypeRequest>
{
    public AircraftTypeRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
        RuleFor(x => x.IataCode).NotEmpty().MaximumLength(5);
        RuleFor(x => x.IcaoCode).MaximumLength(4);
        RuleFor(x => x.SizeCategory).MaximumLength(1);
        RuleFor(x => x.NoiseCategory).MaximumLength(10);
        RuleFor(x => x.Manufacturer).MaximumLength(100);
        RuleFor(x => x.Model).MaximumLength(100);
    }
}
public sealed class AircraftTypeMetadataMap : IMetadataMap<AircraftType, AircraftTypeRequest, AircraftTypeResponse>
{
    public string GetCode(AircraftType entity) => entity.Id;

    public AircraftType Create(AircraftTypeRequest r) => AircraftType.Create(r.Code);

    public void Apply(AircraftType e, AircraftTypeRequest r) => e.Update(
        r.IataCode,r.IataSubCode, r.IcaoCode, r.SizeCategory, r.NoiseCategory, r.Manufacturer, r.Model,
        r.Mtow, r.LandingWeight, r.AvgSeatCapacity, r.IsBridgeRequired);

    public AircraftTypeResponse ToResponse(AircraftType e) => new(
        e.Id, e.IataCode, e.IataSubCode, e.IcaoCode, e.SizeCategory, e.NoiseCategory, e.Manufacturer,
        e.Model, e.Mtow, e.LandingWeight, e.SeatCapacity, e.IsBridgeRequired);

    public Expression<Func<AircraftType, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<AircraftType, AircraftTypeResponse>> Projection { get; } = e => new AircraftTypeResponse(
        e.Id, e.IataCode, e.IataSubCode, e.IcaoCode, e.SizeCategory, e.NoiseCategory, e.Manufacturer,
        e.Model, e.Mtow, e.LandingWeight, e.SeatCapacity, e.IsBridgeRequired);
}
