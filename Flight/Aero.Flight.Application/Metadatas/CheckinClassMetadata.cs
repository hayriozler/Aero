using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class CheckinClassRequestValidator : AbstractValidator<CheckinClassRequest>
{
    public CheckinClassRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(50);
    }
}

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
