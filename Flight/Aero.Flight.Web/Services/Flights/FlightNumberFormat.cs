namespace Aero.Flights.Web.Services.Flights;

internal static class FlightNumberFormat
{
    public const int MaxLength = 8;

    public static string? Validate(string flightNumber)
    {
        if (string.IsNullOrWhiteSpace(flightNumber))
            return null;

        var trimmed = flightNumber.Trim();
        if (!TrySplit(trimmed, out var code, out var number))
            return "Enter a code followed by a number, e.g. \"TK 2563\".";

        if (code.Length > 3)
            return "The code portion can be at most 3 characters.";
        if (number.Length > 5 || !number.All(char.IsDigit))
            return "The number portion must be up to 5 digits.";

        return null;
    }

    public static string Normalize(string? flightNumber)
    {
        if (string.IsNullOrWhiteSpace(flightNumber))
            return flightNumber ?? string.Empty;

        var trimmed = flightNumber.Trim();
        if (!TrySplit(trimmed, out var code, out var number))
            return trimmed;

        code = code.ToUpperInvariant();
        return code.Length + number.Length < 8 ? $"{code} {number}" : $"{code}{number}";
    }

    // The flight number is always the trailing run of digits — scanning from the end (rather than
    // for the first digit from the start) is required because real IATA airline codes can
    // themselves start with a digit (9W, 6E, 5X, 3U, 4O, 8M, 2P, ...).
    private static bool TrySplit(string trimmed, out string code, out string number)
    {
        var i = trimmed.Length;
        while (i > 0 && char.IsDigit(trimmed[i - 1]))
            i--;

        if (i == 0 || i == trimmed.Length)
        {
            code = "";
            number = "";
            return false;
        }

        code = trimmed[..i].TrimEnd();
        number = trimmed[i..];
        return true;
    }
}
