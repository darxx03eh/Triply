using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Domain.Enums.HotleImages;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the hotel repository.</summary>
/// <summary>Persistence operations for hotel.</summary>
public class HotelRepository(TriplyDbContext context, ISieveProcessor sieveProcessor) 
    : GenericRepository<Hotel>(context),
    IHotelRepository
{
    /// <summary>Checks whether the locations exists.</summary>
    public async Task<bool> IsLocationsExistsAsync(decimal? latitude, decimal? longitude,
        CancellationToken cancellationToken)
        => await context.Hotels
            .AnyAsync(h => h.Latitude == latitude && h.Longitude == longitude, cancellationToken);

    /// <summary>Checks whether the location exists exclude identifier.</summary>
    public async Task<bool> IsLocationExistsExcludeIdAsync(decimal? latitude, decimal? longitude, Guid hotelId,
        CancellationToken cancellationToken = default)
        => await context.Hotels
            .AnyAsync(h => h.Latitude == latitude && h.Longitude == longitude && h.HotelId != hotelId,
                cancellationToken);

    /// <summary>Checks whether the hotel identifier exists.</summary>
    public async Task<bool> IsHotelIdExistsAsync(Guid hotelId, CancellationToken cancellationToken = default)
        => await context.Hotels.AnyAsync(h => h.HotelId == hotelId, cancellationToken);

    /// <summary>Gets the thumbnails.</summary>
    public async Task<Dictionary<Guid, string>> GetThumbnailsAsync(IEnumerable<Guid> hotelIds,
        CancellationToken cancellationToken = default)
        => await context.HotelImages
            .Where(i => hotelIds.Contains(i.HotelId) && i.Status == HotelImageStatus.Uploaded && i.Url != null)
            .GroupBy(i => i.HotelId)
            .Select(g => new
            {
                HotelId = g.Key,
                Url = g.OrderBy(i => i.DisplayOrder).Select(i => i.Url).First()
            })
            .ToDictionaryAsync(x => x.HotelId, x => x.Url!, cancellationToken);

    /// <summary>Gets the rooms count.</summary>
    public async Task<Dictionary<Guid, int>> GetRoomsCountAsync(IEnumerable<Guid> hotelIds,
        CancellationToken cancellationToken = default)
        => await context.Rooms
            .Where(r => hotelIds.Contains(r.HotelId))
            .GroupBy(r => r.HotelId)
            .Select(g => new { HotelId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.HotelId, x => x.Count, cancellationToken);

    /// <summary>Checks whether the hotel exists.</summary>
    public async Task<bool> IsHotelExistsAsync(string name, Guid cityId, CancellationToken cancellationToken)
        => await context.Hotels.AnyAsync(h => h.Name.ToUpper() == name.ToUpper() && h.CityId == cityId,
            cancellationToken);

    /// <summary>Checks whether the hotel exists exclude identifier.</summary>
    public async Task<bool> IsHotelExistsExcludeId(string name, Guid cityId, Guid hotelId,
        CancellationToken cancellationToken = default)
        => await context.Hotels.AnyAsync(h =>
                (h.Name.ToUpper() == name.ToUpper() && h.CityId == cityId)
                && h.HotelId != hotelId,
            cancellationToken);

    /// <summary>Gets the hotel by its identifier including the images.</summary>
    public async Task<Hotel?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
    => await context.Hotels
        .Include(h => h.City)
        .Include(h => h.Images
            .Where(i => i.Status == HotelImageStatus.Uploaded && i.Url != null)
            .OrderBy(i => i.DisplayOrder))
        .Include(h => h.HotelAmenities)
            .ThenInclude(ha => ha.Amenity)
        .FirstOrDefaultAsync(h => h.HotelId == id, cancellationToken);

    /// <summary>Gets a paginated list of hotels.</summary>
    public async Task<(List<Hotel> Hotels, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var query = context.Hotels
            .Include(h => h.City)
            .Include(h => h.Owner)
            .AsQueryable();

        if (isAdmin) query = query.IgnoreQueryFilters();

        query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var totalCount = await query.CountAsync(cancellationToken);

        query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
        var hotels = await query.ToListAsync(cancellationToken);

        return (hotels, totalCount);
    }

    /// <summary>Softs the delete rooms.</summary>
    public async Task SoftDeleteRoomsAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        var rooms = await context.Rooms
            .Where(r => r.HotelId == hotelId)
            .ToListAsync(cancellationToken);

        foreach (var room in rooms)
        {
            room.IsDeleted = true;
            room.ModifiedAt = DateTime.UtcNow;
        }
    }

    /// <summary>Sets the original row version.</summary>
    public void SetOriginalRowVersion(Hotel hotel, byte[] rowVersion)
        => context.Entry(hotel).Property(c => c.RowVersion).OriginalValue = rowVersion;

    /// <summary>Gets the review stats.</summary>
    public async Task<(decimal? AverageRating, int ReviewsCount)> GetReviewStatsAsync(Guid hotelId,
        CancellationToken cancellationToken = default)

    {
        var stats = await context.Reviews
            .Where(r => r.HotelId == hotelId)
            .GroupBy(r => r.HotelId)
            .Select(g => new
            {
                Average = g.Average(r => (decimal)r.Rating),
                Count =  g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return stats is null ? (null, 0) : (Math.Round(stats.Average, 1), stats.Count);
    }
}