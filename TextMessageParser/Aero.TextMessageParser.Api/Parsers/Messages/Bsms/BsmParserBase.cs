using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.ValueObjects;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

public abstract class BsmParserBase<TResult>(
    string? changeOfStatus,
    IReadOnlyDictionary<string, Element> elementMap)
    : MessageParser<TResult>(new MessageHeader(Consts.BSM, null, changeOfStatus), elementMap)
    where TResult : class
{
    public override IReadOnlyList<ElementSequence> ElementSequences { get; } =
    [
        new(Consts.V, ElementRequirement.Mandatory,   IsMultiple: false),
        new(Consts.F, ElementRequirement.Conditional, IsMultiple: false),
        new(Consts.I, ElementRequirement.Conditional, IsMultiple: true),
        new(Consts.O, ElementRequirement.Conditional, IsMultiple: true),
        new(Consts.N, ElementRequirement.Mandatory,   IsMultiple: true),
        new(Consts.D, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.S, ElementRequirement.Conditional, IsMultiple: false),
        new(Consts.H, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.W, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.P, ElementRequirement.Conditional, IsMultiple: true),
        new(Consts.G, ElementRequirement.Optional,    IsMultiple: true),
        new(Consts.Y, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.C, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.L, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.T, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.K, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.E, ElementRequirement.Optional,    IsMultiple: true),
        new(Consts.R, ElementRequirement.Optional,    IsMultiple: true),
        new(Consts.X, ElementRequirement.Optional,    IsMultiple: false),
    ];

    protected override ParseError? ValidateResults(IReadOnlyList<ElementResult> results)
        => FlightRules.RequireOutboundDestination(results);
}
