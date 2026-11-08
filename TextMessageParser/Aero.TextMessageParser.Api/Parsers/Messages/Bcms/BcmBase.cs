using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.ValueObjects;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bcms;

public abstract class BcmBase(
    string secondaryIdentifier,
    IReadOnlyDictionary<string, Element> elementMap)
    : MessageBase<TextMessageBcmDto>(new MessageHeader(Consts.BCM, secondaryIdentifier, null), elementMap)
{
    private readonly string _subType = char.ToUpperInvariant(secondaryIdentifier[0]) + secondaryIdentifier[1..].ToLowerInvariant();

    public override IReadOnlyList<ElementSequence> ElementSequences =>
    [
        new(Consts.V, ElementRequirement.Mandatory, IsMultiple: false),
        new(Consts.F, ElementRequirement.Mandatory, IsMultiple: false),
        new(Consts.H, ElementRequirement.Optional,  IsMultiple: false),
        new(Consts.R, ElementRequirement.Optional,  IsMultiple: false),
    ];

    protected override TextMessageBcmDto Handle(long messageId, ElementResult[] elementResults)
    {
        char? ackRequest = null;
        char? sourceIndicator = null;
        var sourceAirport = string.Empty;
        var airlineCode = string.Empty;
        var flightNumber = string.Empty;
        var flightIataDate = string.Empty;

        foreach (var result in elementResults)
        {
            switch (result.Element)
            {
                case ElementF f:
                    airlineCode = f.AirlineCode;
                    flightNumber = f.FlightNumber;
                    flightIataDate = f.FlightDate;
                    break;
                case ElementV v:
                    ackRequest = v.AckRequest;
                    sourceIndicator = v.BaggageSourceIndicator;
                    sourceAirport = v.AirportCode;
                    break;
            }
        }

        return new TextMessageBcmDto(
            messageId, _subType, ackRequest, sourceIndicator, sourceAirport,
            airlineCode, flightNumber, flightIataDate);
    }
}
