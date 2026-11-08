using System.Text.RegularExpressions;

namespace AeroFlight.Application.Flights;

// A flight can have multiple gates/counters/belts assigned (stored as a delimited raw string,
// e.g. "101,102,103,104,105"). Consecutive numeric codes sharing the same alpha prefix/suffix
// collapse into a range ("101-105"); a single code passes through unchanged ("203").
internal static partial class ResourceRangeFormatter
{
    [GeneratedRegex(@"^(?<prefix>\D*)(?<number>\d+)(?<suffix>\D*)$")]
    private static partial Regex TokenPattern();

    public static string? Format(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return raw;

        var tokens = raw.Split([',', ';', '-'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length <= 1)
            return raw.Trim();

        var parsed = new List<(string Prefix, int Number, string Suffix)>(tokens.Length);
        foreach (var token in tokens)
        {
            var match = TokenPattern().Match(token);
            if (!match.Success)
                return string.Join(", ", tokens);

            parsed.Add((match.Groups["prefix"].Value, int.Parse(match.Groups["number"].Value), match.Groups["suffix"].Value));
        }

        var parts = new List<string>();
        foreach (var group in parsed.GroupBy(p => (p.Prefix, p.Suffix)).OrderBy(g => g.Key.Prefix))
        {
            var numbers = group.Select(p => p.Number).Distinct().OrderBy(n => n).ToList();
            foreach (var (start, end) in BuildRuns(numbers))
            {
                parts.Add(start == end
                    ? $"{group.Key.Prefix}{start}{group.Key.Suffix}"
                    : $"{group.Key.Prefix}{start}{group.Key.Suffix}-{group.Key.Prefix}{end}{group.Key.Suffix}");
            }
        }

        return string.Join(", ", parts);
    }

    private static List<(int Start, int End)> BuildRuns(List<int> sortedNumbers)
    {
        var runs = new List<(int Start, int End)>();
        var start = sortedNumbers[0];
        var prev = start;

        foreach (var n in sortedNumbers.Skip(1))
        {
            if (n == prev + 1)
            {
                prev = n;
                continue;
            }

            runs.Add((start, prev));
            start = n;
            prev = n;
        }

        runs.Add((start, prev));
        return runs;
    }
}
