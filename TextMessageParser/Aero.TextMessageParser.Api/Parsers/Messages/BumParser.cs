using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Parsers.Messages.Bsms;
using Aero.TextMessageParser.Api.ValueObjects;

namespace Aero.TextMessageParser.Api.Parsers.Messages;

public sealed class BumParser(IReadOnlyDictionary<string, Element> elementMap)
    : MessageParser<BagRemovalDto>(new MessageHeader(Consts.BUM, null, null), elementMap)
{
    public override IReadOnlyList<ElementSequence> ElementSequences { get; } =
    [
        new(Consts.V, ElementRequirement.Mandatory,   IsMultiple: false),
        new(Consts.K, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.F, ElementRequirement.Mandatory, IsMultiple: false),
        new(Consts.U, ElementRequirement.Optional,    IsMultiple: true),
        new(Consts.N, ElementRequirement.Mandatory,   IsMultiple: true),
        new(Consts.I, ElementRequirement.Optional, IsMultiple: false),
        new(Consts.O, ElementRequirement.Optional, IsMultiple: true),
        new(Consts.Q, ElementRequirement.Conditional, IsMultiple: true),
        new(Consts.S, ElementRequirement.Optional,    IsMultiple: true),
        new(Consts.H, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.W, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.P, ElementRequirement.Conditional, IsMultiple: true),
        new(Consts.Y, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.C, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.L, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.T, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.E, ElementRequirement.Optional,    IsMultiple: true),
        new(Consts.R, ElementRequirement.Optional,    IsMultiple: true),
        new(Consts.X, ElementRequirement.Optional,    IsMultiple: false),
    ];

    protected override BagRemovalDto Handle(long messageId, ElementResult[] elementResults)
        => BsmMapper.ToBagRemoval(messageId, elementResults, "BUM");

    protected override ParseError? ValidateResults(IReadOnlyList<ElementResult> results)
        => FlightRules.RequireOutboundDestination(results);
}
