using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages;

internal static class FlightRules
{
    public static ParseError? RequireOutboundDestination(IEnumerable<ElementResult> results)
    {
        foreach (var result in results)
            if (result.Element is OutboundFlightElement { Destination.Length: 0 })
                return ParseError.ForElement(ErrorCodes.OUTBOUND_FLIGHT_NOT_CORRECT, "OutboundFlightElement destination is required in this message", ".F");
        return null;
    }
}
