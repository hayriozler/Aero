namespace Infrastructure.Diagnostics;

public sealed record CapturedQuery(string CommandType, string Sql, TimeSpan Duration);

public sealed class QueryCollector
{
    private readonly List<CapturedQuery> _queries = [];

    public IReadOnlyList<CapturedQuery> Queries => _queries;
    public TimeSpan TotalDuration => TimeSpan.FromTicks(_queries.Sum(q => q.Duration.Ticks));

    public void Capture(string commandType, string sql, TimeSpan duration) =>
        _queries.Add(new CapturedQuery(commandType, sql, duration));
}
