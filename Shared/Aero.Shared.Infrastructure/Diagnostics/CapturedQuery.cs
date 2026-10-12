namespace Aero.Shared.Infrastructure.Diagnostics;

public sealed record CapturedQuery(string CommandType, string Sql, TimeSpan Duration);
