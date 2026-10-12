using AeroFlight.Domain.Common;

namespace AeroFlight.Infrastructure.Persistence.Audit;

internal sealed record AuditRule(
    Type TargetLogType,
    Func<object, string> ExtractTargetKey,
    string ActionPrefix);
