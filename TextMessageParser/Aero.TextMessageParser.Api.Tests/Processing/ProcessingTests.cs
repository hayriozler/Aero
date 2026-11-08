using Aero.TextMessageParser.Api.Services;
using System.Text.RegularExpressions;
using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Parsers.Messages;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Aero.TextMessageParser.Api.Tests.Processing;

public class ProcessingTests
{
    private const string ProcessingServicePath = "Services/MessageProcessingService.cs";

    [Fact(DisplayName = "Bug 8 (reproduction, skipped): a message whose processing throws must end in a persisted terminal state", Skip = "Needs PostgreSQL: ProcessBatchAsync claims rows with raw UPDATE ... FOR UPDATE SKIP LOCKED ... RETURNING, which neither InMemory nor SQLite can run, and the method is private. Reproduction: insert a BUM TextMessage (Bum.Handle throws NotImplementedException) into a real PostgreSQL schema 'messages', run the service for one batch, then read the row: expected Status ERROR (or similar), Completed true and a TextMessageError row; observed Status stays PROCESSING and Completed false until stale recovery resets it after 2 minutes, so it is retried forever.")]
    [Trait("Bug", "8")]
    public void Failed_message_is_persisted_as_terminal()
    {
    }

    [Fact(DisplayName = "Bug 8 (source guard): the per-message catch block must call RecordFailureAsync, which persists Status, Completed and an error row")]
    [Trait("Bug", "8")]
    [Trait("Kind", "SourceGuard")]
    public void Catch_block_persists_failure()
    {
        const string Why = "If the catch block only changes the entity in memory, the row stays PROCESSING with Completed false, is recovered as stale and retried forever. The catch must call RecordFailureAsync and that helper must write Completed = true, an error row and call SaveChangesAsync. Static source guard because the claim query needs PostgreSQL.";
        var source = SourceLocator.ReadApiSource(ProcessingServicePath);
        var marker = source.IndexOf("Failed to process message", StringComparison.Ordinal);
        Check.True(marker > 0, "Log statement of the per-message catch block not found; update this guard.");
        var catchStart = source.LastIndexOf("catch (Exception ex)", marker, StringComparison.Ordinal);
        var catchBody = source.Substring(catchStart, marker - catchStart + 300);
        Check.True(catchBody.Contains("RecordFailureAsync"), "The catch block does not call RecordFailureAsync. " + Why);
        var helperStart = source.IndexOf("private async Task RecordFailureAsync", StringComparison.Ordinal);
        Check.True(helperStart > 0, "RecordFailureAsync not found. " + Why);
        var helperBody = source.Substring(helperStart);
        Check.True(helperBody.Contains("MarkFailed") && helperBody.Contains("TextMessageErrorSet.Add") && helperBody.Contains("SaveChangesAsync"),
            "RecordFailureAsync must call TextMessage.MarkFailed (which sets Status ERROR and Completed), add a TextMessageError and call SaveChangesAsync. " + Why);
    }

    [Fact(DisplayName = "Bug 8 (source guard): stale PROCESSING rows are recovered periodically inside the loop, not only once at startup")]
    [Trait("Bug", "8")]
    [Trait("Kind", "SourceGuard")]
    public void Stale_recovery_runs_inside_the_loop()
    {
        var source = SourceLocator.ReadApiSource(ProcessingServicePath);
        var loopStart = source.IndexOf("while (!stoppingToken.IsCancellationRequested)", StringComparison.Ordinal);
        Check.True(loopStart > 0, "Main loop not found; update this guard.");
        Check.True(source.Substring(loopStart).Contains("RecoverStaleRecordsAsync"),
            "RecoverStaleRecordsAsync is only called before the loop, so rows left in PROCESSING by a crash or cancellation stay stuck until the next restart. Static source guard because the service needs PostgreSQL.");
    }

    [Fact(DisplayName = "Bug 8 (current behavior): Bum.Handle throws NotImplementedException, which is what reaches the catch block")]
    [Trait("Bug", "8")]
    public void Bum_handler_throws_not_implemented()
    {
        var raw = SampleMessages.Build("BUM", null, SampleMessages.NativeNewLine, SampleMessages.V, SampleMessages.N1);
        var bum = new Bum(ElementMapFactory.Create());
        using var db = TestDbContextFactory.Create();
        var ex = Record.Exception(() => bum.ProcessMessage(TextMessages.From(raw), db));
        Check.True(ex is NotImplementedException, $"Documents today's behavior; observed {ex?.GetType().Name ?? "no exception"}.");
    }

    [Fact(DisplayName = "Bug 9 (reproduction, skipped): SqlQueryRaw<long> over 'RETURNING \"Id\"' must work", Skip = "Needs PostgreSQL. SqlQueryRaw<long> maps a single column that must be named \"Value\"; the statement returns a column named \"Id\", so EF fails with 'The required column Value was not present in the results of a FromSql operation'. Reproduction: run ProcessBatchAsync against PostgreSQL with one NOT_PROCESSED row. InMemory cannot run raw SQL and SQLite lacks the 'messages' schema and FOR UPDATE SKIP LOCKED.")]
    [Trait("Bug", "9")]
    public void Raw_sql_returns_value_column()
    {
    }

    [Fact(DisplayName = "Bug 9 (source guard): the claim SQL must alias the returned id as \"Value\"")]
    [Trait("Bug", "9")]
    [Trait("Kind", "SourceGuard")]
    public void Claim_sql_aliases_id_as_value()
    {
        var source = SourceLocator.ReadApiSource(ProcessingServicePath).Replace("\"\"", "\"");
        Check.True(Regex.IsMatch(source, "RETURNING\\s+\"Id\"\\s+AS\\s+\"Value\"", RegexOptions.IgnoreCase),
            "SqlQueryRaw<long> requires the scalar column to be named Value; the statement ends with RETURNING \"Id\" without an alias. Static source guard because Postgres is not available.");
    }

    [Fact(DisplayName = "Bug 14: the DbContext model must contain an entity for parsed results beyond TextMessages, MessageErrors and ElementErrors")]
    [Trait("Bug", "14")]
    public void Model_contains_parsed_result_storage()
    {
        using var db = TestDbContextFactory.Create();
        var apiAssembly = typeof(TextMessage).Assembly;
        var known = new[] { typeof(TextMessage), typeof(TextMessageError), typeof(ElementError) };
        var extra = db.Model.GetEntityTypes()
            .Where(e => !e.IsOwned() && e.ClrType.Assembly == apiAssembly && !known.Contains(e.ClrType))
            .Select(e => e.ClrType.Name)
            .ToList();
        Check.True(extra.Count > 0,
            "The service is meant to store parsed baggage data in its own database, but the model only has TextMessages, MessageErrors and ElementErrors; parsed DTOs are only POSTed elsewhere. The schema decision belongs to the owner; this test stays red until some parsed-result entity exists.");
    }

    [Fact(DisplayName = "Baseline: the DbContext model builds on InMemory and contains the three known entities")]
    [Trait("Kind", "Baseline")]
    public void Model_contains_known_entities()
    {
        using var db = TestDbContextFactory.Create();
        var names = db.Model.GetEntityTypes().Select(e => e.ClrType).ToList();
        Check.True(names.Contains(typeof(TextMessage)) && names.Contains(typeof(TextMessageError)) && names.Contains(typeof(ElementError)),
            "TextMessage, TextMessageError and ElementError must be mapped.");
    }
}
