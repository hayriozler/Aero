using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Infrastructure.Persistence;

public sealed class PostgresRetryStrategy(
    ExecutionStrategyDependencies dependencies,
    ILogger<PostgresRetryStrategy> logger,
    int maxRetryCount,
    TimeSpan maxRetryDelay)
    : NpgsqlRetryingExecutionStrategy(dependencies, maxRetryCount, maxRetryDelay, errorCodesToAdd: null)
{
    // Connection and transaction errors — safe to retry
    private static readonly HashSet<string> _transientCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "08000", // connection_exception
        "08001", // sqlclient_unable_to_establish_sqlconnection
        "08003", // connection_does_not_exist
        "08004", // sqlserver_rejected_establishment_of_sqlconnection
        "08006", // connection_failure
        "08007", // transaction_resolution_unknown
        "08P01", // protocol_violation
        "57P01", // admin_shutdown
        "57P02", // crash_shutdown
        "57P03", // cannot_connect_now
        "40001", // serialization_failure
        "40P01", // deadlock_detected
        "53300", // too_many_connections
        "53400", // configuration_limit_exceeded
    };

    // Data-integrity errors — never retry, surface immediately
    private static readonly HashSet<string> _nonTransientCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "23000", // integrity_constraint_violation
        "23001", // restrict_violation
        "23502", // not_null_violation
        "23503", // foreign_key_violation
        "23505", // unique_violation  (duplicate key / PK conflict)
        "23514", // check_violation
        "23P01", // exclusion_violation
        "22001", // string_data_right_truncation
        "22003", // numeric_value_out_of_range
        "42601", // syntax_error
        "42703", // undefined_column
        "42P01", // undefined_table
        "42501", // insufficient_privilege
    };

    protected override bool ShouldRetryOn(Exception? exception)
    {
        var sqlState = ExtractSqlState(exception);

        if (sqlState is not null)
        {
            if (_nonTransientCodes.Contains(sqlState))
            {
                logger.LogDebug(
                    "Postgres error {SqlState} is non-transient — not retrying. {Message}",
                    sqlState, exception!.Message);
                return false;
            }

            if (_transientCodes.Contains(sqlState))
            {
                logger.LogWarning(
                    "Postgres transient error {SqlState} — will retry (attempt {Attempt}/{Max}). {Message}",
                    sqlState, ExceptionsEncountered.Count + 1, RetriesOnFailure, exception!.Message);
                return true;
            }

            // Unknown Postgres code: fall back to Npgsql's IsTransient flag
            var isTransient = base.ShouldRetryOn(exception);
            if (isTransient)
            {
                logger.LogWarning(
                    "Postgres error {SqlState} flagged as transient by driver — will retry (attempt {Attempt}/{Max}). {Message}",
                    sqlState, ExceptionsEncountered.Count + 1, RetriesOnFailure, exception!.Message);
            }
            else
            {
                logger.LogDebug(
                    "Postgres error {SqlState} is non-transient (driver decision) — not retrying. {Message}",
                    sqlState, exception!.Message);
            }
            return isTransient;
        }

        // Non-Postgres exception (socket/timeout/etc.) — defer to base
        var baseDecision = base.ShouldRetryOn(exception);
        if (baseDecision)
        {
            logger.LogWarning(
                "Non-Postgres exception will be retried (attempt {Attempt}/{Max}): {ExType} — {Message}",
                ExceptionsEncountered.Count + 1, RetriesOnFailure,
                exception?.GetType().Name, exception?.Message);
        }
        return baseDecision;
    }

    private static string? ExtractSqlState(Exception? exception)
    {
        if (exception is PostgresException pgEx)
            return pgEx.SqlState;

        // SqlState may be buried in an inner exception
        if (exception?.InnerException is PostgresException inner)
            return inner.SqlState;

        return null;
    }
}

