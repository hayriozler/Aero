using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class DelayReasonRequestValidator : AbstractValidator<DelayReasonRequest>
{
    public DelayReasonRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(3);
        RuleFor(x => x.AlphabeticCode).MaximumLength(3);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}

public sealed class DelayReasonMetadataMap : IMetadataMap<DelayReason, DelayReasonRequest, DelayReasonResponse>
{
    public string GetCode(DelayReason entity) => entity.Id;

    public DelayReason Create(DelayReasonRequest r) => DelayReason.Create(r.Code, r.AlphabeticCode, r.Description);

    public void Apply(DelayReason e, DelayReasonRequest r) => e.Update(r.AlphabeticCode, r.Description);

    public DelayReasonResponse ToResponse(DelayReason e) => new(
        e.Id, e.AlphabeticCode, e.Description);

    public Expression<Func<DelayReason, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<DelayReason, DelayReasonResponse>> Projection { get; } = e => new DelayReasonResponse(
        e.Id, e.AlphabeticCode, e.Description);
}
