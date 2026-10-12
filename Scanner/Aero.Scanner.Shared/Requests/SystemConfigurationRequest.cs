namespace Aero.Scanner.Shared.Requests;

public sealed record CreateSystemConfigurationRequest(string Key, string Value, string? Description);
public sealed record UpdateSystemConfigurationRequest(string? Value, string? Description);