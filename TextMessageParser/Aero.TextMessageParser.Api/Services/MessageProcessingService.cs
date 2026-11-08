using Aero.Shared.Infrastructure.Services;
using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aero.TextMessageParser.Api.Services;

public class MessageProcessingService(
    LoggerService<MessageProcessingService> logger,
    IServiceScopeFactory serviceFactory,
    IOptions<MessageProcessingOptions> optionsAccessor) : BackgroundService
{
    private const string ClaimSql = @"
                UPDATE ""messages"".""TextMessages""
                SET ""Status"" = '" + TextMessageStatusStore.Processing + @"', ""ProcessingStartedAt"" = {0}, ""AttemptCount"" = ""AttemptCount"" + 1
                WHERE ""Id"" IN (
                    SELECT ""Id"" FROM ""messages"".""TextMessages""
                    WHERE ""Status"" = '" + TextMessageStatusStore.NotProcessed + @"'
                    ORDER BY ""ReceivedAtUtc""
                    LIMIT {1}
                    FOR UPDATE SKIP LOCKED
                )
                RETURNING ""Id"" AS ""Value""";

    private readonly MessageProcessingOptions _options = optionsAccessor.Value;

    private async Task RecoverStaleRecordsAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TextMessageParserDbContext>();
        var cutoff = DateTime.UtcNow - _options.StaleThreshold;
        var maxAttempts = _options.MaxAttempts;

        var exhausted = await dbContext.TextMessages
            .Where(m => m.Status == TextMessageStatus.Processing
                     && m.ProcessingStartedAt < cutoff
                     && m.AttemptCount >= maxAttempts)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.Status, TextMessageStatus.Error)
                .SetProperty(m => m.ProcessedAtUtc, (DateTime?)DateTime.UtcNow),
                cancellationToken);

        var recovered = await dbContext.TextMessages
            .Where(m => m.Status == TextMessageStatus.Processing
                     && m.ProcessingStartedAt < cutoff)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.Status, TextMessageStatus.NotProcessed)
                .SetProperty(m => m.ProcessingStartedAt, (DateTime?)null),
                cancellationToken);

        if (exhausted > 0)
            logger.LogWarning("Marked {Count} stale PROCESSING records as ERROR after {MaxAttempts} attempts", exhausted, maxAttempts);
        if (recovered > 0)
            logger.LogWarning("Recovered {Count} stale PROCESSING records", recovered);
    }

    private async Task<int> ProcessBatchAsync(CancellationToken cancellationToken)
    {
        List<long> claimedIds;
        using (var scope = serviceFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TextMessageParserDbContext>();
            claimedIds = await dbContext.Database
                .SqlQueryRaw<long>(ClaimSql, DateTime.UtcNow, _options.BatchSize)
                .ToListAsync(cancellationToken);
        }

        if (claimedIds.Count == 0)
        {
            logger.LogDebug("No messages found for processing");
            return 0;
        }

        logger.LogInfo("Processing batch of {Count} messages", claimedIds.Count);

        await Parallel.ForEachAsync(
            claimedIds,
            new ParallelOptions { MaxDegreeOfParallelism = _options.MaxParallelism, CancellationToken = cancellationToken },
            ProcessOneAsync);

        return claimedIds.Count;
    }

    private async ValueTask ProcessOneAsync(long messageId, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TextMessageParserDbContext>();
            var message = await dbContext.TextMessages.FirstAsync(m => m.Id == messageId, cancellationToken);

            var isSuccess = await ProcessMessageAsync(message, scope.ServiceProvider, dbContext, cancellationToken);
            if (isSuccess)
            {
                message.MarkProcessed(DateTime.UtcNow);
                logger.LogInfo("Message {Id} processed successfully", message.Id);
            }
            else
            {
                message.MarkFailed(DateTime.UtcNow);
                logger.LogInfo("Message {Id} processed with errors", message.Id);
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to process message {Id}", messageId);
            await RecordFailureAsync(messageId, ex, cancellationToken);
        }
    }

    private async Task RecordFailureAsync(long messageId, Exception exception, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TextMessageParserDbContext>();
            var message = await dbContext.TextMessages.FirstAsync(m => m.Id == messageId, cancellationToken);

            var retry = FailureClassifier.IsTransient(exception) && message.TryRelease(_options.MaxAttempts);
            if (!retry)
                message.MarkFailed(DateTime.UtcNow);
            else
                logger.LogWarning("Message {Id} will be retried, attempt {Attempt} of {MaxAttempts}", messageId, message.AttemptCount, _options.MaxAttempts);

            var description = $"{exception.GetType().Name}: {exception.Message}";
            if (description.Length > 250) description = description[..250];
            dbContext.MessageErrors.Add(TextMessageError.Create(
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
            textParserDataContext.MessageErrors.Add(TextMessageError.Create(
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
                    nextRecoveryAt = DateTime.UtcNow + _options.RecoveryInterval;
                }
                var claimed = await ProcessBatchAsync(stoppingToken);
                if (claimed >= _options.BatchSize)
                    continue;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Unhandled error in message processing batch");
            }

            await Task.Delay(_options.PollInterval, stoppingToken);
        }

        logger.LogInfo("MessageProcessingService stopped");
    }
}
