using AeroFlight.Contracts.Topology;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AeroFlight.Infrastructure.Messaging;

internal sealed class DebugMessageSinkService(IMessageBus messageBus, ILogger<DebugMessageSinkService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageBus.DeclareSinkAsync("debug.metadata", ExchangeNames.Metadata, "#", maxQueueLength: 1000, stoppingToken);
        await messageBus.DeclareSinkAsync("debug.flights", ExchangeNames.Flights, "#", maxQueueLength: 1000, stoppingToken);

        logger.LogInformation("Debug sink queues bound: debug.metadata, debug.flights.");
    }
}
