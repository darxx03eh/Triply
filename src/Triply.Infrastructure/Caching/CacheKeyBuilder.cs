using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Application.Extensions;

namespace Triply.Infrastructure.Caching;

/// <summary>Builds cache keys used by the caching service.</summary>
public static class CacheKeyBuilder
{
    /// <summary>Builds a versioned, hashed cache key for the given group and key.</summary>
    /// <param name="group">The cache group the key belongs to.</param>
    /// <param name="version">The current version of the cache group.</param>
    /// <param name="key">The raw key to hash.</param>
    public static string Build(string group, long version, string key)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)));
        return $"triply:{group}:v{version}:{hash}";
    }

    /// <summary>Builds the key used to store the current version of the given cache group.</summary>
    /// <param name="group">The cache group.</param>
    public static string BuildVersionKey(string group)
        => $"triply:{group}:version";

    /// <summary>Builds a raw cache key by serializing the given value to JSON.</summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to serialize into a key.</param>
    public static string BuildKey<T>(T value)
        => JsonSerializer.Serialize(value);

    /// <summary>Builds a deterministic key for a paginated city query.</summary>
    /// <remarks>
    /// The request model binds omitted query values as null while the city service applies page 1 and page size 10.
    /// Canonicalizing those defaults prevents equivalent requests from creating different cache entries.
    /// </remarks>
    public static string BuildCitiesKey(GetCitiesRequest request, bool isAdmin)
        => BuildPagedSieveKey(request.Filters, request.Sorts, request.Page, request.PageSize, isAdmin);

    /// <summary>Builds a deterministic key for a Sieve-based paginated query.</summary>
    public static string BuildPagedSieveKey(string? filters, string? sorts, int? page, int? pageSize,
        bool isAdmin, object? scope = null)
        => BuildKey(new PagedSieveCacheKey(
            scope,
            page ?? 1,
            pageSize ?? 10,
            NormalizeSieveValue(filters),
            NormalizeSieveValue(sorts),
            isAdmin));

    /// <summary>Builds a deterministic key from the normalized search criteria.</summary>
    public static string BuildSearchKey(SearchHotelsRequest request)
    {
        var criteria = request.ToCriteria();
        return BuildKey(new SearchCacheKey(
            criteria.Q,
            criteria.CityId,
            criteria.CheckIn,
            criteria.CheckOut,
            criteria.Adults,
            criteria.Children,
            criteria.Rooms,
            criteria.MinPrice,
            criteria.MaxPrice,
            criteria.Stars.Order().ToArray(),
            criteria.Types.Order().ToArray(),
            criteria.Amenities.Order().ToArray(),
            criteria.Sort,
            criteria.Page,
            criteria.PageSize));
    }

    /// <summary>Builds a key for home-page endpoints whose omitted count defaults to five.</summary>
    public static string BuildHomeCountKey(int? count)
        => BuildKey(new { Count = Math.Clamp(count ?? 5, 1, 20) });

    private static string NormalizeSieveValue(string? value)
        => string.Join(',', (value ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

    private sealed record PagedSieveCacheKey(object? Scope, int Page, int PageSize, string Filters, string Sorts,
        bool IsAdmin);

    private sealed record SearchCacheKey(string? Q, Guid? CityId, DateOnly CheckIn, DateOnly CheckOut,
        int Adults, int Children, int Rooms, decimal? MinPrice, decimal? MaxPrice, byte[] Stars,
        Array Types, Guid[] Amenities, string Sort, int Page, int PageSize);
}
