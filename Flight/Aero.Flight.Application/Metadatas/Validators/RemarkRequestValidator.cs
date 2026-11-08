using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class RemarkRequestValidator : AbstractValidator<RemarkRequest>
{
    public RemarkRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Description).MaximumLength(20);
    }
}
