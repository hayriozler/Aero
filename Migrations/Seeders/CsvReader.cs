namespace Aero.Migrations.Seeders;

internal static class CsvReader
{
    public static List<Dictionary<string, string?>> ReadRows(StreamReader reader, bool parseHeaderRow)
    {
        var headerLine = reader.ReadLine();
        if (headerLine is null) return [];

        var headers = parseHeaderRow ? ParseRow(headerLine) : headerLine.Split(',');
        var rows = new List<Dictionary<string, string?>>();

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var values = ParseRow(line);
            var row = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < headers.Length; i++)
            {
                var v = i < values.Length ? values[i] : null;
                row[headers[i]] = string.IsNullOrEmpty(v) || string.Equals(v, "NULL", StringComparison.OrdinalIgnoreCase) ? null : v;
            }
            rows.Add(row);
        }

        return rows;
    }

    public static string[] ParseRow(string line)
    {
        var fields = new List<string>();
        var i = 0;

        while (i <= line.Length)
        {
            if (i == line.Length)
            {
                fields.Add(string.Empty);
                break;
            }

            if (line[i] == '"')
            {
                i++;
                var sb = new System.Text.StringBuilder();
                while (i < line.Length)
                {
                    if (line[i] == '"' && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i += 2;
                    }
                    else if (line[i] == '"')
                    {
                        i++;
                        break;
                    }
                    else
                    {
                        sb.Append(line[i++]);
                    }
                }
                fields.Add(sb.ToString());
                if (i < line.Length && line[i] == ',') i++;
            }
            else
            {
                var start = i;
                while (i < line.Length && line[i] != ',') i++;
                fields.Add(line[start..i]);
                if (i < line.Length) i++; // skip comma
            }
        }

        return [.. fields];
    }
}
