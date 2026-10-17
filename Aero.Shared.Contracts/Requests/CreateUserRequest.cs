namespace Aero.Shared.Contracts.Requests;

public sealed record CreateUserRequest(
    string Username,
    string DisplayName,
    string Password,
    List<string> RoleNames);
