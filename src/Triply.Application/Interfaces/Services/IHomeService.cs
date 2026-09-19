using Triply.Application.DTOs.Deals;
using Triply.Application.DTOs.Home;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

public interface IHomeService
{
    Task RecordVisitAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<RecentHotelResponse>>> GetRecentHotelsAsync(Guid userId, int? count,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<TrendingCityResponse>>> GetTrendingCitiesAsync(int? count,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<FeaturedDealResponse>>> GetFeaturedDealsAsync(int? count,
        CancellationToken cancellationToken = default);
}