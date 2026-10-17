namespace Aero.Flights.Contracts.Metadatas;

public sealed record CompanyRequest(
    string Code, string? Name, string? DisplayName, string? CompanyTypeCode,
    string? TaxNumber, string? BillingAddress, string? BankAccount, string? InvoiceCurrency);

public sealed record CompanyResponse(
    string Code, string? Name, string? DisplayName, string? CompanyTypeCode,
    string? TaxNumber, string? BillingAddress, string? BankAccount, string? InvoiceCurrency);
