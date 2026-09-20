using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Triply.Infrastructure.Settings;

namespace Triply.Infrastructure.Interceptors;

/// <summary>Gets or sets the slow query interceptor.</summary>
/// <summary>Interceptor that logs the slow query.</summary>
public class SlowQueryInterceptor(
    IOptions<DatabaseLoggingSettings> settings,
    ILogger<SlowQueryInterceptor> logger) : DbCommandInterceptor
{
    /// <summary>Readers the executed.</summary>
    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
        DbDataReader result, CancellationToken cancellationToken = default)
    {
        LogIfSlow(command, eventData);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    /// <summary>Readers the executed.</summary>
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData,
        DbDataReader result)
    {
        LogIfSlow(command, eventData);
        return base.ReaderExecuted(command, eventData, result);
    }

    /// <summary>Nons the query executed.</summary>
    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
        int result, CancellationToken cancellationToken = default)
    {
        LogIfSlow(command, eventData);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    /// <summary>Nons the query executed.</summary>
    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        LogIfSlow(command, eventData);
        return base.NonQueryExecuted(command, eventData, result);
    }

    /// <summary>Scalars the executed.</summary>
    public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData,
        object? result, CancellationToken cancellationToken = default)
    {
        LogIfSlow(command, eventData);
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    /// <summary>Scalars the executed.</summary>
    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        LogIfSlow(command, eventData);
        return base.ScalarExecuted(command, eventData, result);
    }

    private void LogIfSlow(DbCommand command, CommandExecutedEventData eventData)
    {
        var elapsed = eventData.Duration.TotalMilliseconds;
        if (elapsed < settings.Value.SlowQueryThresholdMs) return;

        logger.LogWarning("Slow query ({ElapsedMs:0} ms, threshold {SlowQueryThresholdMs} ms) on {DbContext}: {Sql}",
            elapsed, settings.Value.SlowQueryThresholdMs, eventData.Context?.GetType().Name, command.CommandText);
    }
}
