using Triply.Application.DTOs.Home;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Home;

public partial class HomeService
{
    public async Task<Result<IReadOnlyList<TrendingCityResponse>>> GetTrendingCitiesAsync(int? count,
        CancellationToken cancellationToken = default)
    {
        var cities = await recentVisitRepository.GetTrendingCitiesAsync(
            DateTime.UtcNow.AddDays(-30), Math.Clamp(count ?? 5, 1, 20), cancellationToken);

        return Result<IReadOnlyList<TrendingCityResponse>>.Success(cities,
            success: new("TRENDING_CITIES_FOUND", 
                "Trending destinations were retrieved successfully."));
    }
}