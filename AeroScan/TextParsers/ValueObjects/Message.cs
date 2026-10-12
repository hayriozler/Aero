using IataText.Parser.Contracts;

namespace IataText.Parser.ValueObjects;

public sealed record Message(
    MessageHeader Header,
    IReadOnlyList<ElementDetail> Elements,
    MessageFooter Footer);
