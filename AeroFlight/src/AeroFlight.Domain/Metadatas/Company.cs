using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Metadatas;

public sealed class Company : AuditBase<string>
{
    public string? Name { get; private set; }
    public string? DisplayName { get; private set; }
    public string? CompanyTypeCode { get; private set; }
    public string? TaxNumber { get; private set; }
    public string? BillingAddress { get; private set; }
    public string? BankAccount { get; private set; }
    public string? InvoiceCurrency { get; private set; }

    private Company() { }

    private Company(string code, string? name, string? displayName)
    {
        SetId(code);
        Name = name;
        DisplayName = displayName;
    }

    public static Company Create(string code, string? name = null, string? displayName = null)
        => new(code, name, displayName);

    public void Update(
        string? name, string? displayName, string? companyTypeCode,
        string? taxNumber, string? billingAddress, string? bankAccount, string? invoiceCurrency)
    {
        Name = name;
        DisplayName = displayName;
        CompanyTypeCode = companyTypeCode;
        TaxNumber = taxNumber;
        BillingAddress = billingAddress;
        BankAccount = bankAccount;
        InvoiceCurrency = invoiceCurrency;
    }
}
