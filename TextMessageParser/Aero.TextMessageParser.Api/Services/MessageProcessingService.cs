using Aero.Shared.Infrastructure.Services;
using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.TextMessageParser.Api.Services;

public class MessageProcessingService(
    LoggerService<MessageProcessingService> logger,
    IServiceScopeFactory serviceFactory) : BackgroundService
{
    private const string ClaimSql = @"
                UPDATE ""messages"".""TextMessages""
                SET ""Status"" = '" + TextMessageStatus.Processing + @"', ""ProcessingStartedAt"" = {0}
                WHERE ""Id"" IN (
                    SELECT ""Id"" FROM ""messages"".""TextMessages""
                    WHERE ""Status"" = '" + TextMessageStatus.NotProcessed + @"' AND ""Completed"" = false
                    ORDER BY ""RecordDateTime""
                    LIMIT {1}
                    FOR UPDATE SKIP LOCKED
                )
                RETURNING ""Id"" AS ""Value""";

    private const int _batchSize = 20;
    private static readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan _staleThreshold = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan _recoveryInterval = TimeSpan.FromMinutes(1);

    private async Task RecoverStaleRecordsAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TextMessageParserDbContext>();
        var cutoff = DateTime.UtcNow - _staleThreshold;

        var recovered = await dbContext.TextMessagesSet
            .Where(m => m.Status == TextMessageStatus.Processing
                     && m.Completed == false
                     && m.ProcessingStartedAt < cutoff)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.Status, TextMessageStatus.NotProcessed)
                .SetProperty(m => m.ProcessingStartedAt, (DateTime?)null),
                cancellationToken);

        if (recovered > 0)
            logger.LogWarning("Recovered {Count} stale PROCESSING records", recovered);
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceFactory.CreateScope();
        var textParserDataContext = scope.ServiceProvider.GetRequiredService<TextMessageParserDbContext>();
        var claimedAt = DateTime.UtcNow;

        var claimedIds = await textParserDataContext.Database
            .SqlQueryRaw<long>(ClaimSql, claimedAt, _batchSize)
            .ToListAsync(cancellationToken);

        if (claimedIds.Count == 0)
        {
            logger.LogDebug("No messages found for processing");
            return;
        }

        var messages = await textParserDataContext.TextMessagesSet
            .Where(m => claimedIds.Contains(m.Id))
            .ToListAsync(cancellationToken);

        logger.LogInfo("Processing batch of {Count} messages", messages.Count);

        foreach (var msg in messages)
        {
            try
            {
                var isSuccess = await ProcessMessageAsync(msg, scope.ServiceProvider, textParserDataContext, cancellationToken);
                if (isSuccess)
                {
                    msg.MarkProcessed(DateTime.UtcNow);
                    logger.LogInfo("Message {Id} processed successfully", msg.Id);
                }
                else
                {
                    msg.MarkFailed(DateTime.UtcNow);
                    logger.LogInfo("Message {Id} processed with errors", msg.Id);
                }
                await textParserDataContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Failed to process message {Id}", msg.Id);
                DiscardPendingChanges(textParserDataContext, msg);
                await RecordFailureAsync(msg.Id, ex, cancellationToken);
            }
        }
    }

    private static void DiscardPendingChanges(TextMessageParserDbContext dbContext, TextMessage message)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList())
            entry.State = EntityState.Detached;
        dbContext.Entry(message).State = EntityState.Unchanged;
    }

    private async Task RecordFailureAsync(long messageId, Exception exception, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TextMessageParserDbContext>();
            var message = await dbContext.TextMessagesSet.FirstAsync(m => m.Id == messageId, cancellationToken);
            message.MarkFailed(DateTime.UtcNow);

            var description = $"{exception.GetType().Name}: {exception.Message}";
            if (description.Length > 250) description = description[..250];
            dbContext.TextMessageErrorSet.Add(TextMessageError.Create(
                messageId, ErrorCodes.UNHANDLED_PROCESSING_ERROR, description));
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception recordingFailure) when (recordingFailure is not OperationCanceledException)
        {
            logger.LogError(recordingFailure, "Could not record the failure of message {Id}", messageId);
        }
    }

    private async Task<bool> ProcessMessageAsync(
        TextMessage message,
        IServiceProvider scopedServices,
        TextMessageParserDbContext textParserDataContext,
        CancellationToken cancellationToken)
    {
        var key = message.Header.RoutingKey;

        var processor = scopedServices.GetKeyedService<IMessageProcessor>(key);
        if (processor is null)
        {
            textParserDataContext.TextMessageErrorSet.Add(TextMessageError.Create(
                message.Id,
                ErrorCodes.NO_MSG_REGISTERED_HANDLER,
                $"No handler registered for message type {key}"));
            logger.LogError("No handler registered for message type '{Key}'", key);
            return false;
        }

        return await processor.ProcessAsync(message, cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInfo("MessageProcessingService started");

        var nextRecoveryAt = DateTime.UtcNow;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (DateTime.UtcNow >= nextRecoveryAt)
                {
                    await RecoverStaleRecordsAsync(stoppingToken);
                    nextRecoveryAt = DateTime.UtcNow + _recoveryInterval;
                }
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Unhandled error in message processing batch");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        logger.LogInfo("MessageProcessingService stopped");
    }
}
