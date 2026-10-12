namespace AeroFlight.Application.Resources;

public sealed record GetGanttItemsQuery(string ResourceType, DateOnly FromDate, DateOnly ToDate);
