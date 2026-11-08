namespace Aero.Shared.Contracts.Requests;

public sealed record UpdateUserRequest(
    string DisplayName,
    List<string> RoleNames);
