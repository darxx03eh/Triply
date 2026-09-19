using Triply.Application.DTOs.Home;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

public interface IRecentVisitRepository : IGenericRepository<UserRecentVisit>
{
    Task<UserRecentVisit?> GetUserVisitAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken = default);

    Task<List<RecentHotelResponse>> GetRecentHotelsAsync(Guid userId, int count,
        CancellationToken cancellationToken = default);

    Task<List<TrendingCityResponse>> GetTrendingCitiesAsync(DateTime since, int count,
        CancellationToken cancellationToken = default);
}