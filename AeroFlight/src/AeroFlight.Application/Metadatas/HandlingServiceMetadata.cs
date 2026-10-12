using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class HandlingServiceRequestValidator : AbstractValidator<HandlingServiceRequest>
{
    public HandlingServiceRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(15);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}

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
