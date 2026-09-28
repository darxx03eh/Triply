namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the caching operations.</summary>
public interface ICacheService
{
    /// <summary>Gets the cached value for the given group and key, or creates it using <paramref name="factory"/> if it is not cached.</summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="group">The group the key belongs to.</param>
    /// <param name="key">The cache key.</param>
    /// <param name="ttl">The duration the value stays cached before it expires.</param>
    /// <param name="factory">The factory used to create the value when it is not already cached.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<T> GetOrCreateAsync<T>(string group, string key, TimeSpan ttl,
        Func<CancellationToken, Task<T>> factory, CancellationToken cancellationToken = default);

    /// <summary>Invalidates all cached values belonging to the given group.</summary>
    /// <param name="group">The group whose cached values should be invalidated.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task InvalidateGroupAsync(string group, CancellationToken cancellationToken = default);
}