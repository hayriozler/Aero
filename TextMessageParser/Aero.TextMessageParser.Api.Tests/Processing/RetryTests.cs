using System.Data.Common;
using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Services;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using Aero.TextMessageParser.Api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Aero.TextMessageParser.Api.Tests.Processing;

public class RetryTests
{
    private static TextMessage Claimed(int attempts)
    {
        var message = TextMessage.Create("BSM\n.V/7LFRA\nENDBSM", new MessageHeader("BSM", null, null), new MessageFooter("ENDBSM"));
        typeof(TextMessage).GetProperty(nameof(TextMessage.Status))!.SetValue(message, TextMessageStatus.Processing);
        typeof(TextMessage).GetProperty(nameof(TextMessage.AttemptCount))!.SetValue(message, attempts);
        typeof(TextMessage).GetProperty(nameof(TextMessage.ProcessingStartedAt))!.SetValue(message, DateTime.UtcNow);
        return message;
    }

    [Fact(DisplayName = "Retry: a claimed message under the attempt limit is released back to NOT_PROCESSED")]
    [Trait("Bug", "B10")]
    public void Release_under_limit()
    {
        var message = Claimed(attempts: 1);
        Check.True(message.TryRelease(3), "Release must succeed on attempt 1 of 3.");
        Check.Equal(TextMessageStatus.NotProcessed, message.Status, "Status after release.");
        Check.True(message.ProcessingStartedAt is null, "Claim time is cleared.");
    }

    [Fact(DisplayName = "Retry: a claimed message that used all attempts is not released")]
    [Trait("Bug", "B10")]
    public void No_release_at_limit()
    {
        var message = Claimed(attempts: 3);
        Check.True(!message.TryRelease(3), "Release must be refused on attempt 3 of 3.");
        Check.Equal(TextMessageStatus.Processing, message.Status, "Status is unchanged so the caller can mark it failed.");
    }

    [Fact(DisplayName = "Retry: releasing a message that is not claimed throws")]
    [Trait("Bug", "B10")]
    public void Release_requires_claim()
    {
        var message = TextMessage.Create("BSM\n.V/7LFRA\nENDBSM", new MessageHeader("BSM", null, null), new MessageFooter("ENDBSM"));
        var ex = Record.Exception(() => message.TryRelease(3));
        Check.True(ex is InvalidOperationException, "Only PROCESSING messages can be released.");
    }

    [Fact(DisplayName = "Classifier: database, timeout, IO and HTTP failures are transient; mapping and not-implemented failures are permanent")]
    [Trait("Bug", "B10")]
    public void Classifier_separates_transient_from_permanent()
    {
        Check.True(FailureClassifier.IsTransient(new TimeoutException()), "Timeout.");
        Check.True(FailureClassifier.IsTransient(new IOException()), "IO.");
        Check.True(FailureClassifier.IsTransient(new HttpRequestException()), "HTTP.");
        Check.True(FailureClassifier.IsTransient(new DbUpdateException()), "DbUpdateException.");
        Check.True(!FailureClassifier.IsTransient(new NotImplementedException()), "NotImplemented.");
        Check.True(!FailureClassifier.IsTransient(new InvalidOperationException()), "InvalidOperation.");
    }
}
