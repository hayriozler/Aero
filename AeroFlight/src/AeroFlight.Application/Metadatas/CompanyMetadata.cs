using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

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

public sealed class CompanyMetadataMap : IMetadataMap<Company, CompanyRequest, CompanyResponse>
{
    public string GetCode(Company entity) => entity.Id;

    public Company Create(CompanyRequest r) => Company.Create(r.Code, r.Name, r.DisplayName);

    public void Apply(Company e, CompanyRequest r) => e.Update(
        r.Name, r.DisplayName, r.CompanyTypeCode, r.TaxNumber, r.BillingAddress, r.BankAccount, r.InvoiceCurrency);

    public CompanyResponse ToResponse(Company e) => new(
        e.Id, e.Name, e.DisplayName, e.CompanyTypeCode, e.TaxNumber, e.BillingAddress, e.BankAccount, e.InvoiceCurrency);

    public Expression<Func<Company, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<Company, CompanyResponse>> Projection { get; } = e => new CompanyResponse(
        e.Id, e.Name, e.DisplayName, e.CompanyTypeCode, e.TaxNumber, e.BillingAddress, e.BankAccount, e.InvoiceCurrency);
}
