using Aero.Fids.Infrastructure.Flights;
using System.Collections.Concurrent;

namespace Aero.Fids.Web.Boards;

public interface IFlightBoardStore
{
    void Apply(string tenantId, FidsFlight entry);
    bool TryGetExisting(string tenantId, string flightType, long flightId, out FidsFlight? existing);
    IReadOnlyList<FidsFlight> GetBoard(string tenantId, string flightType);
    IReadOnlyList<FidsFlight> GetAll(string tenantId);
}

public sealed class FlightBoardStore(TimeProvider clock) : IFlightBoardStore
{
    private static readonly TimeSpan _landedGraceWindow = TimeSpan.FromMinutes(45);
    private static readonly TimeSpan _departedGraceWindow = TimeSpan.FromMinutes(30);

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<(string FlightType, long FlightId), BoardEntry>> _byTenant =
        new(StringComparer.OrdinalIgnoreCase);

    public void Apply(string tenantId, FidsFlight entry)
    {
        var board = _byTenant.GetOrAdd(tenantId, _ => new());
        var key = (entry.FlightType, entry.FlightId);
        var now = clock.GetUtcNow();

        // StatusSince only moves forward when the status actually changes — reapplying the same
        // status (e.g. a delta that touched an unrelated field) must not restart its grace window.
        var statusSince = board.TryGetValue(key, out var existing) && existing.Flight.Status == entry.Status
            ? existing.StatusSince
            : now;

        board[key] = new BoardEntry(entry, statusSince);
    }

    public bool TryGetExisting(string tenantId, string flightType, long flightId, out FidsFlight? existing)
    {
        existing = _byTenant.TryGetValue(tenantId, out var board) && board.TryGetValue((flightType, flightId), out var entry)
            ? entry.Flight
            : null;
        return existing is not null;
    }

    public IReadOnlyList<FidsFlight> GetBoard(string tenantId, string flightType) =>
        Filter(tenantId, e => string.Equals(e.Flight.FlightType, flightType, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<FidsFlight> GetAll(string tenantId) => Filter(tenantId, static _ => true);

    private IReadOnlyList<FidsFlight> Filter(string tenantId, Func<BoardEntry, bool> predicate)
    {
        if (!_byTenant.TryGetValue(tenantId, out var board))
            return [];

        var now = clock.GetUtcNow();
        return [.. board.Values
            .Where(predicate)
            .Where(e => !IsSettledPastGraceWindow(e, now))
            .Select(e => e.Flight)
            .OrderBy(f => f.ScheduledTime)];
    }

    private static bool IsSettledPastGraceWindow(BoardEntry entry, DateTimeOffset now)
    {
        var (graceWindow, terminalStatus) = entry.Flight.FlightType == FlightTypes.Arrival
            ? (_landedGraceWindow, InternalFlightStatuses.Landed)
            : (_departedGraceWindow, InternalFlightStatuses.Departed);

        return entry.Flight.Status == terminalStatus && now - entry.StatusSince > graceWindow;
    }

    private readonly record struct BoardEntry(FidsFlight Flight, DateTimeOffset StatusSince);
}
