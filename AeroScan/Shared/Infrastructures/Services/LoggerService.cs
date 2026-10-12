using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

// CA2254: This class is an intentional dynamic-template forwarder — the template varies by design.
#pragma warning disable CA2254

public sealed class LoggerService<T>(ILogger<T> logger)
{
    public void LogInfo(string message, params object?[] args) =>
        logger.LogInformation(message, args);

    public void LogWarning(string message, params object?[] args) =>
        logger.LogWarning(message, args);

    public void LogDebug(string message, params object?[] args) =>
        logger.LogDebug(message, args);

    public void LogError(Exception? ex, string message, params object?[] args) =>
        logger.LogError(ex, message, args);

    public void LogError(string message, params object?[] args) =>
        logger.LogError(message, args);
}

#pragma warning restore CA2254
