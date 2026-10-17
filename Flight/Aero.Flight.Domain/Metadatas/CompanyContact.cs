namespace Aero.Flights.Domain.Metadatas;

public sealed class CompanyContact
{
    public int Id { get; private set; }
    public string CompanyCode { get; private set; } = string.Empty;
    public CompanyContactType ContactType { get; private set; }
    public string Value { get; private set; } = string.Empty;

    private CompanyContact() { }

    private CompanyContact(string companyCode, CompanyContactType contactType, string value)
    {
        CompanyCode = companyCode;
        ContactType = contactType;
        Value = value;
    }

    public static CompanyContact Create(string companyCode, CompanyContactType contactType, string value)
        => new(companyCode, contactType, value);

    public void Update(CompanyContactType contactType, string value)
    {
        ContactType = contactType;
        Value = value;
    }
}
