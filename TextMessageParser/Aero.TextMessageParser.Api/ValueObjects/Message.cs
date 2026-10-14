using Aero.TextMessageParser.Api.Contracts;

namespace Aero.TextMessageParser.Api.ValueObjects;

public sealed record Message(
    MessageHeader Header,
    IReadOnlyList<ElementDetail> Elements,
    MessageFooter Footer);
