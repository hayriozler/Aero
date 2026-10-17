namespace Aero.Flights.Application.Flights;

// The Flight Number is entered/received already carrying its own carrier prefix (e.g. "TK 2411") —
// the internal AirlineCode field is a different code (e.g. "THY") and must never be concatenated
// on top, or the display doubles up as "THY TK 2411". A linked pair strips the second leg's prefix
// when it matches the first leg's, so "TK 2411" + "TK125" displays as "TK 2411/125".
internal static class FlightCodeFormatter
{
    public static string Format(string? arrivalNumber, string? departureNumber)
    {
        if (string.IsNullOrWhiteSpace(arrivalNumber))
            return departureNumber ?? string.Empty;
        if (string.IsNullOrWhiteSpace(departureNumber))
            return arrivalNumber;

        var arrivalPrefix = LeadingPrefix(arrivalNumber);
        var departurePrefix = LeadingPrefix(departureNumber);
        var departureDisplay = departurePrefix is not null && string.Equals(arrivalPrefix, departurePrefix, StringComparison.OrdinalIgnoreCase)
            ? departureNumber[departurePrefix.Length..].TrimStart()
            : departureNumber;

        return $"{arrivalNumber}/{departureDisplay}";
    }

    private static string? LeadingPrefix(string number)
    {
        var i = 0;
        while (i < number.Length && !char.IsDigit(number[i]))
            i++;

        return i == 0 || i == number.Length ? null : number[..i].TrimEnd();
    }
}
