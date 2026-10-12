using Aero.MessageProcessor.TextMessageProcessor.Contracts;
using Aero.MessageProcessor.TextMessageProcessor.Parsers.Elements;
using Aero.MessageProcessor.TextMessageProcessor.ValueObjects;
using Aero.Scanner.Shared.Dtos;

namespace Aero.MessageProcessor.TextMessageProcessor.Parsers.Messages.Bcms;

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
