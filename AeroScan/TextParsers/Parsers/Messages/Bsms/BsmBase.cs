using Contracts.Dtos;
using IataText.Parser.Contracts;
using IataText.Parser.Parsers.Elements;
using IataText.Parser.ValueObjects;

namespace IataText.Parser.Parsers.Messages.Bsms;

public abstract class BsmBase<TResult>(
    string? changeOfStatus,
    IReadOnlyDictionary<string, Element> elementMap)
    : MessageBase<TResult>(new MessageHeader(Consts.BSM, null, changeOfStatus), elementMap)
{
    public override IReadOnlyList<ElementSequence> ElementSequences =>
    [
        new(Consts.V, ElementRequirement.Mandatory,   IsMultiple: false),
        new(Consts.F, ElementRequirement.Conditional, IsMultiple: false),
        new(Consts.I, ElementRequirement.Conditional, IsMultiple: true),
        new(Consts.O, ElementRequirement.Conditional, IsMultiple: true),
        new(Consts.N, ElementRequirement.Mandatory,   IsMultiple: true),
        new(Consts.D, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.A, ElementRequirement.Optional,    IsMultiple: true),
        new(Consts.S, ElementRequirement.Conditional, IsMultiple: false),
        new(Consts.H, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.W, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.P, ElementRequirement.Conditional, IsMultiple: true),
        new(Consts.G, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.Y, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.C, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.L, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.T, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.K, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.E, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.R, ElementRequirement.Optional,    IsMultiple: false),
        new(Consts.X, ElementRequirement.Optional,    IsMultiple: false),
    ];

    // All state is local to the call — no instance fields — safe for singleton reuse across concurrent messages.    
}
