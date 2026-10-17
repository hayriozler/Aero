using Aero.Scanner.Domain.Aggregates.Companies;
using Aero.Shared.Domain.Identity;

namespace Aero.Scanner.Domain.Aggregates.Users;

public sealed class ScannerUser : User
{
    public string CompanyCode { get; private set; } = string.Empty;
    public Company Company { get; private set; } = null!;

    private ScannerUser() { }
    public static ScannerUser Create(string username, string passwordHash, string companyCode, string? realName = null)
    {
        var newScannerUser = (ScannerUser)User.Create(username, passwordHash, realName);
        newScannerUser.CompanyCode = companyCode;
        return newScannerUser;
    }
}
