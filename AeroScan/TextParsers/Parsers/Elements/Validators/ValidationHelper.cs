using IataText.Parser.Contracts;
using IataText.Parser.Extensions;

namespace IataText.Parser.Parsers.Elements.Validators;

/// <summary>
/// Span-only validation helpers — no string allocations.
/// </summary>
public static class ValidationHelper
{
    public static bool ValidateAirlineFlightNumber(ReadOnlyMemory<char> airlineFlightNumberValue, out int airlineLength)
    {
        airlineLength = 0;
        if (airlineFlightNumberValue.Length < 5 || airlineFlightNumberValue.Length > 8) return false;

        var span = airlineFlightNumberValue.Span;
        airlineLength = char.IsDigit(span[2]) ? 2 : 3;

        for (int i = 0; i < airlineLength; i++)
            if (!char.IsLetter(span[i])) return false;

        var flightNo  = airlineFlightNumberValue[airlineLength..];
        var flightSpan = flightNo.Span;
        int digitCount = 0;
        for (int i = 0; i < flightSpan.Length; i++)
        {
            var c = flightSpan[i];
            if (char.IsDigit(c))  
                digitCount++; 
            else 
                return false;
        }
        return digitCount >= 1 && digitCount <= 4;
    }

    public static bool ValidateIataDate(ReadOnlyMemory<char> iataDateValue)
    {
        if (iataDateValue.Length != 5) return false;
        var span = iataDateValue.Span;
        if (!char.IsDigit(span[0]) || !char.IsDigit(span[1])) return false;
        return Consts.ValidMonths.ContainsSpan(span.Slice(2, 3));
    }

    public static bool ValidateAirportCode(ReadOnlyMemory<char> airportCodeValue)
    {
        if (airportCodeValue.Length != 3) return false;
        var span = airportCodeValue.Span;
        return char.IsLetter(span[0]) && char.IsLetter(span[1]) && char.IsLetter(span[2]);
    }
}
