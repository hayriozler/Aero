using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class CompanyRequestValidator : AbstractValidator<CompanyRequest>
{
    public CompanyRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Name).MaximumLength(100);
        RuleFor(x => x.DisplayName).MaximumLength(100);
        RuleFor(x => x.CompanyTypeCode).MaximumLength(10);
        RuleFor(x => x.TaxNumber).MaximumLength(30);
        RuleFor(x => x.BillingAddress).MaximumLength(500);
        RuleFor(x => x.BankAccount).MaximumLength(50);
        RuleFor(x => x.InvoiceCurrency).MaximumLength(3);
    }
}
