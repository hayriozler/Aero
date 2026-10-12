using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class TerminalRequestValidator : AbstractValidator<TerminalRequest>
{
    public TerminalRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}

public sealed class TerminalMetadataMap : IMetadataMap<Terminal, TerminalRequest, TerminalResponse>
{
    public string GetCode(Terminal entity) => entity.Id;

    public Terminal Create(TerminalRequest r) => Terminal.Create(r.Code);

    public void Apply(Terminal e, TerminalRequest r) => e.Update(r.Description);

    public TerminalResponse ToResponse(Terminal e) => new(
        e.Id, e.Description);

    public Expression<Func<Terminal, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<Terminal, TerminalResponse>> Projection { get; } = e => new TerminalResponse(
        e.Id, e.Description);
}
