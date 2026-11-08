using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Extensions;

namespace Aero.TextMessageParser.Api.Parsers.Elements.Validators;

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

        if (!IsAirlineDesignator(span[..airlineLength])) return false;

        var flightNo = airlineFlightNumberValue[airlineLength..];
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

    public static bool IsAirlineDesignator(ReadOnlySpan<char> airline)
    {
        if (airline.Length == 3)
            return char.IsLetter(airline[0]) && char.IsLetter(airline[1]) && char.IsLetter(airline[2]);
        if (airline.Length != 2) return false;
        var hasLetter = char.IsLetter(airline[0]) || char.IsLetter(airline[1]);
        return hasLetter && char.IsLetterOrDigit(airline[0]) && char.IsLetterOrDigit(airline[1]);
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
