namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the rate limit operations.</summary>
public interface IRateLimitService
{
    /// <summary>Increments the request count for the given key within the specified time window.</summary>
    Task<long> IncrementAsync(string key, TimeSpan window, CancellationToken cancellationToken = default);
    /// <summary>Gets the current request count for the given key.</summary>
    Task<long> GetCountAsync(string key, CancellationToken cancellationToken = default);
    /// <summary>Resets the request count for the given key.</summary>
    Task ResetAsync(string key, CancellationToken cancellationToken = default);
}