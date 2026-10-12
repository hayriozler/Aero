using Contracts.Dtos;
using IataText.Parser.Contracts;
using IataText.Parser.Parsers.Elements;

namespace IataText.Parser.Parsers.Messages.Bcms;

public sealed class BcmFcm(IReadOnlyDictionary<string, Element> elementMap)
    : BcmBase(Consts.FCM, elementMap)
{
    public override IReadOnlyList<ElementSequence> ElementSequences =>
    [
        new(Consts.V, ElementRequirement.Mandatory, IsMultiple: false),
        new(Consts.F, ElementRequirement.Mandatory, IsMultiple: false),
    ];

    protected override TextMessageBcmDto Handle(long messageId, ElementResult[] elementResults)
       => ToBcmFomDto(messageId, elementResults);
    internal TextMessageBcmDto ToBcmFomDto(long messageId, ElementResult[] elementResults)
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
            "Fcm",
            ackRequest,
            sourceIndicator,
            sourceAirport,
            airlineCode,
            flightNumber,
            flightIataDate);
    }
}
