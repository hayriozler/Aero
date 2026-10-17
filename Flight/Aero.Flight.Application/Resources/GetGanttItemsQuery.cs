namespace Aero.Flights.Application.Resources;

public sealed record GetGanttItemsQuery(string ResourceType, DateOnly FromDate, DateOnly ToDate);
