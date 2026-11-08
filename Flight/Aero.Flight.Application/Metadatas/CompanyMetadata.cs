using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;


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
