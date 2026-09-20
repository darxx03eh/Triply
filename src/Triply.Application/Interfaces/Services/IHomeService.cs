using Triply.Application.DTOs.Deals;
using Triply.Application.DTOs.Home;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the home operations.</summary>
public interface IHomeService
{
    /// <summary>Records the visit.</summary>
    Task RecordVisitAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken = default);

    /// <summary>Gets the recent hotels.</summary>
    Task<Result<IReadOnlyList<RecentHotelResponse>>> GetRecentHotelsAsync(Guid userId, int? count,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the trending cities.</summary>
    Task<Result<IReadOnlyList<TrendingCityResponse>>> GetTrendingCitiesAsync(int? count,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the featured deals.</summary>
    Task<Result<IReadOnlyList<FeaturedDealResponse>>> GetFeaturedDealsAsync(int? count,
        CancellationToken cancellationToken = default);
}