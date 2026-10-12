using Contracts.Dtos;
using IataText.Parser.Contracts;
using IataText.Parser.Parsers.Elements;

namespace IataText.Parser.Parsers.Messages.Bsms;

public sealed class BsmChg(IReadOnlyDictionary<string, Element> elementMap)
    : BsmBase<TextMessageDepartureBagChgDto>(Consts.CHG, elementMap)
{
    private TextMessageDepartureBagChgDto ToChgDepartureBaggageDto(long messageId, ElementResult[] elementResults)
    {
        char? sourceIndicator = null;
        char? ackRequest = null;
        string sourceAirport = string.Empty;
        string airlineCode = string.Empty;
        string flightNumber = string.Empty;
        string flightIataDate = string.Empty;
        string passengerName = string.Empty;
        string destination = string.Empty;
        string? passengerSecurityNumber = null;
        char passengerProfileStatus = default;
        string passengerSequenceNumber = string.Empty;
        char? passengerClass = default;
        string? seatNumber = null;
        bool authorityToLoad = false;
        bool authorityToTransport = false;
        char passengerStatus;
        bool bagTagStatus = false;
        string? piecesWeightIndicator;
        string? checkedWeight;
        List<(string AirlineCode, string FlightNumber, string FlightIataDate, string Destination, char? Class)> Onwards = [];
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
                    passengerClass = elementF.Class;
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
                case ElementO:
                    var elementO = (ElementO)element.Element;
                    Onwards.Add(new(elementO.AirlineCode,
                        elementO.FlightNumber,
                        elementO.FlightDate,
                        elementO.Destination,
                        elementO.Class));
                    break;
                case ElementP:
                    var elementP = (ElementP)element.Element;
                    passengerName = $"{elementP.PassengerGivenName} {elementP.PassengerSurname}";
                    break;
                case ElementS:
                    var elementS = (ElementS)element.Element;
                    authorityToLoad         = elementS.AuthorityToLoad == 'Y';
                    seatNumber              = elementS.SeatNumber;
                    passengerStatus         = elementS.PassengerStatus;
                    passengerSecurityNumber = elementS.SecurityNumber;
                    passengerSequenceNumber = elementS.SequenceNumber;
                    passengerProfileStatus  = elementS.PassengerProfileStatus;
                    authorityToTransport    = elementS.AuthorityToTransport == 'Y';
                    bagTagStatus            = elementS.BaggageTagStatus == 'A';
                    break;
                case ElementV:
                    var elementV = (ElementV)element.Element;
                    ackRequest      = elementV.AckRequest;
                    sourceIndicator = elementV.BaggageSourceIndicator;
                    sourceAirport   = elementV.AirportCode;
                    break;
                case ElementW:
                    var elementW = (ElementW)element.Element;
                    piecesWeightIndicator = elementW.PiecesWeightIndicator;
                    checkedWeight = elementW.CheckedWeight;
                    break;
            }
        }

        if (sourceIndicator is null)
            throw new InvalidOperationException($"BSM CHG {messageId}: ElementV (SourceIndicator) is required but missing");

        return new TextMessageDepartureBagChgDto(
            messageId,
            ackRequest, authorityToLoad, authorityToTransport,
            sourceIndicator.Value, sourceAirport,
            airlineCode, flightNumber, flightIataDate, destination,
            bagTagStatus, BagTags,
            Onwards,
            passengerProfileStatus, passengerName, passengerSecurityNumber, passengerSequenceNumber, passengerClass,
            seatNumber);
    }
    protected override TextMessageDepartureBagChgDto Handle(long messageId, ElementResult[] elementResults)
         => ToChgDepartureBaggageDto(messageId, elementResults);
}
