using Triply.Application.DTOs.Home;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for recent visit.</summary>
public interface IRecentVisitRepository : IGenericRepository<UserRecentVisit>
{
    /// <summary>Gets the user visit.</summary>
    Task<UserRecentVisit?> GetUserVisitAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken = default);

    /// <summary>Gets the recent hotels.</summary>
    Task<List<RecentHotelResponse>> GetRecentHotelsAsync(Guid userId, int count,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the trending cities.</summary>
    Task<List<TrendingCityResponse>> GetTrendingCitiesAsync(DateTime since, int count,
        CancellationToken cancellationToken = default);
}