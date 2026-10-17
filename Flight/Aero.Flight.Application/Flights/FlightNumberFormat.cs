namespace Aero.Flights.Application.Flights;

internal static class FlightNumberFormat
{
    public const int MaxLength = 8;
    public static bool IsValid(string? flightNumber)
    {
        if (string.IsNullOrWhiteSpace(flightNumber))
            return false;

        var trimmed = flightNumber.Trim();
        if (!TrySplit(trimmed, out var code, out var number))
            return false;

        return code.Length <= 3 && number.Length <= 5 && number.All(char.IsDigit);
    }
    // 3-char code + optional space + 5-digit number
    public static string Normalize(string? flightNumber)
    {
        if (string.IsNullOrWhiteSpace(flightNumber))
            return flightNumber ?? string.Empty;

        var trimmed = flightNumber.Trim();
        if (!TrySplit(trimmed, out var code, out var number))
            return trimmed;

        code = code.ToUpperInvariant();
        return (code.Length + number.Length) < 8 ? $"{code} {number}" : $"{code}{number}";
    }

    private static bool TrySplit(string trimmed, out string code, out string number)
    {
        var i = 0;

        while (i < trimmed.Length && !char.IsDigit(trimmed[i]))
            i++;

        if (i == 0 || i == trimmed.Length)
        {
            code = "";
            number = "";
            return false;
        }

        code = trimmed[..i].TrimEnd(); //Airline Code first 2 or 3 chars
        number = trimmed[i..];
        return true;
    }
}
