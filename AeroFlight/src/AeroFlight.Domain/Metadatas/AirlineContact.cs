namespace AeroFlight.Domain.Metadatas;

public sealed class AirlineContact
{
    public int Id { get; private set; }
    public string AirlineCode { get; private set; } = string.Empty;
    public AirlineContactType ContactType { get; private set; }
    public string Value { get; private set; } = string.Empty;

    private AirlineContact() { }

    private AirlineContact(string airlineCode, AirlineContactType contactType, string value)
    {
        AirlineCode = airlineCode;
        ContactType = contactType;
        Value = value;
    }

    public static AirlineContact Create(string airlineCode, AirlineContactType contactType, string value)
        => new(airlineCode, contactType, value);

    public void Update(AirlineContactType contactType, string value)
    {
        ContactType = contactType;
        Value = value;
    }
}
