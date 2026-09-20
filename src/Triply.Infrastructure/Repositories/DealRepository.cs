using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Triply.Application.DTOs.Deals;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Domain.Enums.HotleImages;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the deal repository.</summary>
/// <summary>Persistence operations for deal.</summary>
public class DealRepository(TriplyDbContext context, ISieveProcessor sieveProcessor) 
    : GenericRepository<Deal>(context),
    IDealRepository
{
    /// <summary>Gets a paginated list of deals.</summary>
    public async Task<(List<Deal> Deals, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        CancellationToken cancellationToken = default)
    {
        var query = context.Deals.AsNoTracking();

        if (string.IsNullOrWhiteSpace(sieveModel.Sorts))
            query = query.OrderByDescending(d => d.CreatedAt);

        query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var totalCount = await query.CountAsync(cancellationToken);

        query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
        var deals = await query.ToListAsync(cancellationToken);

        return (deals, totalCount);
    }

    /// <summary>Gets the featured.</summary>
    public async Task<List<FeaturedDealResponse>> GetFeaturedAsync(int count,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await context.Deals
            .AsNoTracking()
            .Where(d => d.IsFeatured
                        && d.StartsAt <= now
                        && d.EndsAt > now
                        && d.Room.IsAvailable
                        && !d.Room.Hotel.IsDeleted)
            .OrderByDescending(d => d.DiscountPercentage)
            .ThenBy(d => d.EndsAt)
            .Take(count)
            .Select(d => new FeaturedDealResponse
            {
                DealId = d.DealId,
                Title = d.Title,
                HotelId = d.Room.HotelId,
                HotelName = d.Room.Hotel.Name,
                CityName = d.Room.Hotel.City.Name,
                Country = d.Room.Hotel.City.Country,
                StarRating = d.Room.Hotel.StarRating,
                ThumbnailUrl = d.Room.Hotel.Images
                    .Where(i => i.Status == HotelImageStatus.Uploaded && i.Url != null)
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.Url)
                    .FirstOrDefault(),
                RoomId = d.RoomId,
                RoomType = d.Room.RoomType,
                OriginalPrice = d.Room.PricePerNight,
                DiscountedPrice = Math.Round(d.Room.PricePerNight * (100 - d.DiscountPercentage) / 100, 2),
                DiscountPercentage = d.DiscountPercentage,
                EndsAt = d.EndsAt
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>Checks whether the deal overlapping.</summary>
    public async Task<bool> IsDealOverlappingAsync(Guid roomId, DateTime startsAt, DateTime endsAt,
        Guid? excludeDealId = null,
        CancellationToken cancellationToken = default)
        => await context.Deals.AnyAsync(d => d.RoomId == roomId
                                             && d.DealId != excludeDealId
                                             && d.StartsAt < endsAt
                                             && d.EndsAt > startsAt, cancellationToken);
}