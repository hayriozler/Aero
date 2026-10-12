using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class AircraftSizeCategoryRequestValidator : AbstractValidator<AircraftSizeCategoryRequest>
{
    public AircraftSizeCategoryRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
    }
}

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
