using Aero.Scanner.Shared.Requests.TextMessages;
using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bcms;

public sealed class BcmBam(IReadOnlyDictionary<string, Element> elementMap)
    : BcmBase(Consts.BAM, elementMap)
{
    public override IReadOnlyList<ElementSequence> ElementSequences =>
    [
        new(Consts.V, ElementRequirement.Mandatory, IsMultiple: false),
        new(Consts.A, ElementRequirement.Mandatory, IsMultiple: false),
        new(Consts.R, ElementRequirement.Optional,  IsMultiple: false),
    ];

    protected override TextMessageBcmDto Handle(long messageId, ElementResult[] elementResults)
        => throw new NotImplementedException();
}
