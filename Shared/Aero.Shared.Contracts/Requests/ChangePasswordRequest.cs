namespace Aero.Shared.Contracts.Requests;

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
