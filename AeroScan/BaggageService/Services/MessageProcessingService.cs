using BaggageService.Persistence;
using IataText.Parser.Contracts;
using IataText.Parser.Entities;
using IataText.Parser.Parsers;
using IataText.Parser.Persistence;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Services;

public class MessageProcessingService(
    LoggerService<MessageProcessingService> logger,
    IServiceScopeFactory serviceFactory,
    IReadOnlyDictionary<string, IMessageParserDispatcher> dispatcherMap) : BackgroundService
{
    private const int _batchSize = 20;
    private static readonly TimeSpan _interval       = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan _staleThreshold = TimeSpan.FromMinutes(2);

    private async Task RecoverStaleRecordsAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AeroScanDataContext>();
        var cutoff = DateTime.UtcNow - _staleThreshold;

        var recovered = await dbContext.TextMessagesSet
            .Where(m => m.Status == "PROCESSING"
                     && m.Completed == false
                     && m.ProcessingStartedAt < cutoff)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.Status, "NOT_PROCESSED")
                .SetProperty(m => m.ProcessingStartedAt, (DateTime?)null),
                cancellationToken);

        if (recovered > 0)
            logger.LogWarning("Recovered {Count} stale PROCESSING records", recovered);
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AeroScanDataContext>();
        var textParserDataContext = scope.ServiceProvider.GetRequiredService<ITextParserDbContext>();
        var claimedAt = DateTime.UtcNow;

        var claimedIds = await dbContext.Database
            .SqlQueryRaw<long>(@"
                UPDATE ""messages"".""TextMessages""
                SET ""Status"" = 'PROCESSING', ""ProcessingStartedAt"" = {0}
                WHERE ""Id"" IN (
                    SELECT ""Id"" FROM ""messages"".""TextMessages""
                    WHERE ""Status"" = 'NOT_PROCESSED' AND ""Completed"" = false
                    ORDER BY ""RecordDateTime""
                    LIMIT {1}
                    FOR UPDATE SKIP LOCKED
                )
                RETURNING ""Id""", claimedAt, _batchSize)
            .ToListAsync(cancellationToken);

        if (claimedIds.Count == 0)
        {
            logger.LogInfo("No messages found for processing");
            return;
        }

        var messages = await dbContext.TextMessagesSet
            .Where(m => claimedIds.Contains(m.Id))
            .ToListAsync(cancellationToken);

        logger.LogInfo("Processing batch of {Count} messages", messages.Count);

        foreach (var msg in messages)
        {
            try
            {
                await DispatchMessageAsync(msg, textParserDataContext, dbContext, cancellationToken);
                msg.Status = "PROCESSED";
                msg.Completed = true;
                msg.ProcessDateTime = DateTime.UtcNow;
                logger.LogInfo("Message {Id} processed successfully", msg.Id);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                msg.Status = "ERROR";
                logger.LogError(ex, "Failed to process message {Id}", msg.Id);
            }
        }
    }

    private async Task DispatchMessageAsync(
        TextMessage message,
        ITextParserDbContext textParserDataContext,
        IDataContext textParserResultContext,
        CancellationToken cancellationToken)
    {
        var key = message.Header.Identifier + message.Header.SecondaryIdentifier;

        if (!dispatcherMap.TryGetValue(key, out var dispatcher))
        {
            ((AeroScanDataContext)textParserResultContext).TextMessageErrorSet.Add(TextMessageError.Create(
                message.Id,
                ErrorCodes.NO_MSG_REGISTERED_HANDLER,
                $"No handler registered for message type {key}"));
            logger.LogError("No handler registered for message type '{Key}'", key);
            return;
        }

        await dispatcher.DispatchAsync(message, textParserDataContext, textParserResultContext, cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInfo("MessageProcessingService started");

        await RecoverStaleRecordsAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
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
