using Contracts.Dtos;
using IataText.Parser.Contracts;
using IataText.Parser.Parsers.Elements;

namespace IataText.Parser.Parsers.Messages.Bsms;

public sealed class BsmDel(IReadOnlyDictionary<string, Element> elementMap)
    : BsmBase<TextMessageDepartureBagDeleteDto>(Consts.DEL, elementMap)
{
    private TextMessageDepartureBagDeleteDto ToDelDepartureBaggageDto(long messageId, ElementResult[] elementResults)
    {
        char? sourceIndicator = null;
        char? ackRequest = null;
        string sourceAirport = string.Empty;
        string airlineCode = string.Empty;
        string flightNumber = string.Empty;
        string flightIataDate = string.Empty;
        string destination = string.Empty;
        List<string> BagTags = [];

        foreach (var element in elementResults)
        {
            switch (element.Element)
            {
                case ElementF:
                    var elementF = (ElementF)element.Element;
                    airlineCode    = elementF.AirlineCode;
                    flightNumber   = elementF.FlightNumber;
                    flightIataDate = elementF.FlightDate;
                    destination    = elementF.Destination;
                    break;
                case ElementN:
                    var elementN = (ElementN)element.Element;
                    var bagTagNumber = $"{elementN.AirlinePrefix}{elementN.TagSerial}";
                    var consecutiveNumber = int.TryParse(elementN.ConsecutiveTags, out var cn) ? cn : 0;
                    var intBagTagNumber = int.Parse(elementN.TagSerial);
                    for (var i = 0; i < consecutiveNumber; i++)
                    {
                        if (intBagTagNumber > 999999) intBagTagNumber = 1;
                        var tmpIntBaggageTag = intBagTagNumber + i;
                        if (tmpIntBaggageTag > 999999)
                        {
                            tmpIntBaggageTag = (intBagTagNumber + i) - 999999;
                        }
                        BagTags.Add($"{elementN.AirlinePrefix}{tmpIntBaggageTag:D6}");
                    }
                    break;
                case ElementV:
                    var elementV = (ElementV)element.Element;
                    ackRequest      = elementV.AckRequest;
                    sourceIndicator = elementV.BaggageSourceIndicator;
                    sourceAirport   = elementV.AirportCode;
                    break;
            }
        }

        if (sourceIndicator is null)
            throw new InvalidOperationException($"BSM DEL {messageId}: ElementV (SourceIndicator) is required but missing");

        return new TextMessageDepartureBagDeleteDto(
            messageId,
            ackRequest,
            sourceIndicator.Value, sourceAirport,
            airlineCode, flightNumber, flightIataDate, destination,
            BagTags);
    }
    protected override TextMessageDepartureBagDeleteDto Handle(long messageId, ElementResult[] elementResults)
    => ToDelDepartureBaggageDto(messageId, elementResults);
}
