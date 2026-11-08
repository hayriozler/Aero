namespace Aero.Fids.Application.Statuses;

public sealed record StatusMappingResponse(long Id, string Dimension, string? FlightType, string ExternalValue, string InternalValue);
