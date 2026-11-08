using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class TerminalRequestValidator : AbstractValidator<TerminalRequest>
{
    public TerminalRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Description).MaximumLength(200);
    }
}
