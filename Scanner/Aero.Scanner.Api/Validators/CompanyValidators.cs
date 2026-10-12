using Aero.Scanner.Domain.Enums;
using Aero.Scanner.Shared.Requests;
using FluentValidation;

namespace Aero.Scanner.Api.Validators;

public sealed class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => Enum.TryParse<CompanyType>(t, ignoreCase: true, out _))
            .WithMessage("Invalid company type. Expected: AirportOperator | HandlingAgent");
    }
}

public sealed class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => Enum.TryParse<CompanyType>(t, ignoreCase: true, out _))
            .WithMessage("Invalid company type. Expected: AirportOperator | HandlingAgent");
    }
}
