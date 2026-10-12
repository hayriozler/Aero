namespace Aero.Scanner.Shared.Requests;

public sealed record CreateUserRequest(
    string Username,
    string DisplayName,
    string Password,
    string CompanyCode,
    List<string> RoleNames);

public sealed record UpdateUserRequest(
    string DisplayName,
    List<string> RoleNames);

public sealed record AdminResetPasswordRequest(string NewPassword);
