using Aero.Scanner.Shared.Requests.TextMessages;
using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.ValueObjects;

namespace Aero.TextMessageParser.Api.Parsers.Messages.Bcms;

public abstract class BcmBase(
    string secondaryIdentifier,
    IReadOnlyDictionary<string, Element> elementMap)
    : MessageBase<TextMessageBcmDto>(new MessageHeader(Consts.BCM, secondaryIdentifier, null), elementMap)
{
    public override IReadOnlyList<ElementSequence> ElementSequences =>
    [
        new(Consts.V, ElementRequirement.Mandatory, IsMultiple: false),
        new(Consts.F, ElementRequirement.Mandatory, IsMultiple: false),
        new(Consts.H, ElementRequirement.Optional,  IsMultiple: false),
        new(Consts.R, ElementRequirement.Optional,  IsMultiple: false),
    ];
}
