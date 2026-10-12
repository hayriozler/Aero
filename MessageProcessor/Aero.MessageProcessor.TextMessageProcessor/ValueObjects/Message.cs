using Aero.MessageProcessor.TextMessageProcessor.Contracts;

namespace Aero.MessageProcessor.TextMessageProcessor.ValueObjects;

public sealed record Message(
    MessageHeader Header,
    IReadOnlyList<ElementDetail> Elements,
    MessageFooter Footer);
