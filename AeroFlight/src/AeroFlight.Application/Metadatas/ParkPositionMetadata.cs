using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class ParkPositionRequestValidator : AbstractValidator<ParkPositionRequest>
{
    public ParkPositionRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.TypeCode).MaximumLength(10);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}

public sealed class ParkPositionMetadataMap : IMetadataMap<ParkPosition, ParkPositionRequest, ParkPositionResponse>
{
    public string GetCode(ParkPosition entity) => entity.Id;

    public ParkPosition Create(ParkPositionRequest r) => ParkPosition.Create(r.Code);

    public void Apply(ParkPosition e, ParkPositionRequest r) => e.Update(
        r.TypeCode, r.Description, r.Capacity,
        r.Latitude, r.Longitude, r.Angle);

    public ParkPositionResponse ToResponse(ParkPosition e) => new(
        e.Id, e.TypeCode, e.Description, e.Capacity,
        e.Latitude, e.Longitude, e.Angle);

    public Expression<Func<ParkPosition, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<ParkPosition, ParkPositionResponse>> Projection { get; } = e => new ParkPositionResponse(
        e.Id, e.TypeCode, e.Description, e.Capacity,
        e.Latitude, e.Longitude, e.Angle);
}
