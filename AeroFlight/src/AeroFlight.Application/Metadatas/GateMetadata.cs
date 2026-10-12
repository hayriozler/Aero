using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class GateRequestValidator : AbstractValidator<GateRequest>
{
    public GateRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.GateTypeCode).MaximumLength(10);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}

public sealed class GateMetadataMap : IMetadataMap<Gate, GateRequest, GateResponse>
{
    public string GetCode(Gate entity) => entity.Id;

    public Gate Create(GateRequest r) => Gate.Create(r.Code, r.GateTypeCode);

    public void Apply(Gate e, GateRequest r) =>
        e.Update(r.GateTypeCode, r.TerminalCode, r.Description, r.Capacity, r.HasLoadingBridge, r.HasPca);

    public GateResponse ToResponse(Gate e) => new(
        e.Id, e.GateTypeCode, e.TerminalCode, e.Description, e.Capacity,
        e.HasLoadingBridge, e.HasPca);

    public Expression<Func<Gate, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<Gate, GateResponse>> Projection { get; } = e => new GateResponse(
        e.Id, e.GateTypeCode, e.TerminalCode, e.Description, e.Capacity,
        e.HasLoadingBridge, e.HasPca);
}
