using Aero.Scanner.Shared.Requests.TextMessages;
using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.ValueObjects;

namespace Aero.TextMessageParser.Api.Parsers.Messages;

public sealed class Bum(IReadOnlyDictionary<string, Element> elementMap)
    : MessageBase<TextMessageBumRequest>(new MessageHeader(Consts.BUM, null, null), elementMap)
{
    public override IReadOnlyList<ElementSequence> ElementSequences =>
    [
        new(Consts.V, ElementRequirement.Mandatory,   IsMultiple: false),
        new(Consts.K, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.F, ElementRequirement.Conditional, IsMultiple: false),
        new(Consts.U, ElementRequirement.Optional,    IsMultiple: true),
        new(Consts.N, ElementRequirement.Mandatory,   IsMultiple: true),
        new(Consts.I, ElementRequirement.Conditional, IsMultiple: false),
        new(Consts.O, ElementRequirement.Conditional, IsMultiple: false),
        new(Consts.Q, ElementRequirement.Conditional, IsMultiple: false),
        new(Consts.S, ElementRequirement.Conditional, IsMultiple: false),
        new(Consts.H, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.W, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.P, ElementRequirement.Conditional, IsMultiple: false),
        new(Consts.Y, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.C, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.L, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.T, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.E, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.R, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.X, ElementRequirement.Optional,    IsMultiple: false),
    ];

    protected override TextMessageBumRequest Handle(long messageId, ElementResult[] elementResults)
        => throw new NotImplementedException();
}
