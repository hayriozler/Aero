using Aero.MessageProcessor.TextMessageProcessor.Contracts;
using Aero.MessageProcessor.TextMessageProcessor.Parsers.Elements;
using Aero.Scanner.Shared.Dtos;

namespace Aero.MessageProcessor.TextMessageProcessor.Parsers.Messages.Bcms;

public sealed class BcmFom(IReadOnlyDictionary<string, Element> elementMap)
    : BcmBase(Consts.FOM, elementMap)
{
    protected override TextMessageBcmDto Handle(long messageId, ElementResult[] elementResults)
    => ToBcmFomDto(messageId, elementResults);
    internal static TextMessageBcmDto ToBcmFomDto(long messageId, ElementResult[] elementResults)
    {
        char? ackRequest = null, sourceIndicator = null;
        string sourceAirport = string.Empty;
        string airlineCode = string.Empty;
        string flightNumber = string.Empty;
        string flightIataDate = string.Empty;

        foreach (var element in elementResults)
        {
            switch (element.Element)
            {
                case ElementF:
                    var elementF = (ElementF)element.Element;
                    airlineCode    = elementF.AirlineCode;
                    flightNumber   = elementF.FlightNumber;
                    flightIataDate = elementF.FlightDate;
                    break;
                case ElementV:
                    var elementV = (ElementV)element.Element;
                    ackRequest      = elementV.AckRequest;
                    sourceIndicator = elementV.BaggageSourceIndicator;
                    sourceAirport   = elementV.AirportCode;
                    break;
            }
        }
        return new TextMessageBcmDto(
            messageId,
            "Fom",
            ackRequest,
            sourceIndicator,
            sourceAirport,
            airlineCode,
            flightNumber,
            flightIataDate);
    }
}
