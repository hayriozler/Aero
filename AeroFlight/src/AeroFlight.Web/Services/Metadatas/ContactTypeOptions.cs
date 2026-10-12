namespace AeroFlight.Web.Services.Metadatas;

// Shared by AirlineContactsPanel/CompanyContactsPanel — matches the AirlineContactType/
// CompanyContactType enum members 1:1 (AeroFlight.Domain.Metadatas), but Web doesn't reference
// Domain at all (goes through Contracts/HTTP only), so this is a plain string list kept in sync by
// hand rather than derived from those enums directly.
public static class ContactTypeOptions
{
    public static readonly string[] All = ["Phone", "Email", "Sita"];
}
