using Microsoft.EntityFrameworkCore;
using Triply.Application.DTOs.Home;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Domain.Enums.HotleImages;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the recent visit repository.</summary>
/// <summary>Persistence operations for recent visit.</summary>
public class RecentVisitRepository(TriplyDbContext context) : GenericRepository<UserRecentVisit>(context),
    IRecentVisitRepository
{
    /// <summary>Gets the user visit.</summary>
    public async Task<UserRecentVisit?> GetUserVisitAsync(Guid userId, Guid hotelId,
        CancellationToken cancellationToken = default)
        => await context.UserRecentVisits
            .FirstOrDefaultAsync(v => v.UserId == userId && v.HotelId == hotelId, cancellationToken);

    /// <summary>Gets the recent hotels.</summary>
    public async Task<List<RecentHotelResponse>> GetRecentHotelsAsync(Guid userId, int count,
        CancellationToken cancellationToken = default)
        => await context.UserRecentVisits
            .AsNoTracking()
            .Where(v => v.UserId == userId && !v.Hotel.IsDeleted)
            .OrderByDescending(v => v.VisitedAt)
            .Take(count)
            .Select(v => new RecentHotelResponse
            {
                HotelId = v.HotelId,
                Name = v.Hotel.Name,
                CityName = v.Hotel.City.Name,
                Country = v.Hotel.City.Country,
                StarRating = v.Hotel.StarRating,
                ThumbnailUrl = v.Hotel.Images
                    .Where(i => i.Status == HotelImageStatus.Uploaded && i.Url != null)
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.Url)
                    .FirstOrDefault(),
                MinPricePerNight = v.Hotel.Rooms
                    .Where(r => r.IsAvailable)
                    .Min(r => (decimal?)r.PricePerNight),
                VisitedAt = v.VisitedAt
            })
            .ToListAsync(cancellationToken);

    /// <summary>Gets the trending cities.</summary>
    public async Task<List<TrendingCityResponse>> GetTrendingCitiesAsync(DateTime since, int count,
        CancellationToken cancellationToken = default)
        => await context.Cities
            .AsNoTracking()
            .Select(c => new TrendingCityResponse
            {
                CityId = c.CityId,
                Name = c.Name,
                Country = c.Country,
                ThumbnailUrl = c.ThumbnailUrl,
                VisitsCount = context.UserRecentVisits
                    .Count(v => v.Hotel.CityId == c.CityId && v.VisitedAt >= since),
                HotelsCount = c.Hotels.Count()
            })
            .OrderByDescending(c => c.VisitsCount)
            .ThenByDescending(c => c.HotelsCount)
            .ThenBy(c => c.Name)
            .Take(count)
            .ToListAsync(cancellationToken);
}