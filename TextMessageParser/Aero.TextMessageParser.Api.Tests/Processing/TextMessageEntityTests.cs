using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using Aero.TextMessageParser.Api.ValueObjects;

namespace Aero.TextMessageParser.Api.Tests.Processing;

public class TextMessageEntityTests
{
    private static TextMessage NewMessage() =>
        TextMessage.Create("BSM\n.V/7LFRA\nENDBSM", new MessageHeader("BSM", null, null), new MessageFooter("ENDBSM"));

    private static void Claim(TextMessage message)
    {
        typeof(TextMessage).GetProperty(nameof(TextMessage.Status))!.SetValue(message, TextMessageStatus.Processing);
    }

    [Fact(DisplayName = "Entity: a new TextMessage starts as NOT_PROCESSED, not completed, with a received timestamp")]
    [Trait("Kind", "Baseline")]
    public void New_message_state()
    {
        var before = DateTime.UtcNow;
        var message = NewMessage();
        Check.Equal(TextMessageStatus.NotProcessed, message.Status, "Initial status.");
        Check.True(!message.Completed, "A new message is not completed.");
        Check.True(message.ProcessedAtUtc is null, "A new message has no process time.");
        Check.True(message.ReceivedAtUtc >= before && message.ReceivedAtUtc <= DateTime.UtcNow, "ReceivedAtUtc is set by Create.");
    }

    [Fact(DisplayName = "Entity: MarkProcessed on a claimed message sets PROCESSED, Completed and the process time")]
    public void Mark_processed_completes_a_claimed_message()
    {
        var message = NewMessage();
        Claim(message);
        var at = new DateTime(2026, 11, 8, 12, 0, 0, DateTimeKind.Utc);
        message.MarkProcessed(at);
        Check.Equal(TextMessageStatus.Processed, message.Status, "Status.");
        Check.True(message.Completed, "Completed.");
        Check.True(message.ProcessedAtUtc == at, "ProcessedAtUtc.");
    }

    [Fact(DisplayName = "Entity: MarkFailed on a claimed message sets ERROR, Completed and the process time")]
    public void Mark_failed_completes_a_claimed_message()
    {
        var message = NewMessage();
        Claim(message);
        var at = new DateTime(2026, 11, 8, 12, 0, 0, DateTimeKind.Utc);
        message.MarkFailed(at);
        Check.Equal(TextMessageStatus.Error, message.Status, "Status.");
        Check.True(message.Completed, "Completed.");
        Check.True(message.ProcessedAtUtc == at, "ProcessedAtUtc.");
    }

    [Fact(DisplayName = "Entity: a message that was never claimed (or was already completed) cannot be completed again")]
    public void Completing_requires_a_claimed_message()
    {
        var unclaimed = NewMessage();
        Check.True(Record.Exception(() => unclaimed.MarkProcessed(DateTime.UtcNow)) is InvalidOperationException,
            "A NOT_PROCESSED message was never claimed, so completing it would hide a processing-order bug.");
        var done = NewMessage();
        Claim(done);
        done.MarkProcessed(DateTime.UtcNow);
        Check.True(Record.Exception(() => done.MarkFailed(DateTime.UtcNow)) is InvalidOperationException,
            "A completed message must not change its outcome.");
    }
}
