using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bcms;

public sealed class BcmFcmParser(IReadOnlyDictionary<string, Element> elementMap)
    : BcmParserBase(Consts.FCM, elementMap)
{
    public override IReadOnlyList<ElementSequence> ElementSequences { get; } =
    [
        new(Consts.V, ElementRequirement.Mandatory, IsMultiple: false),
        new(Consts.F, ElementRequirement.Mandatory, IsMultiple: false),
    ];
}
