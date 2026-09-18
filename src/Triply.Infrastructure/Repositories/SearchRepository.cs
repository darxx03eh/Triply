using Microsoft.EntityFrameworkCore;
using Triply.Application.DTOs.Search;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Enums.HotleImages;
using Triply.Infrastructure.Db;

namespace Triply.Infrastructure.Repositories;

public class SearchRepository(TriplyDbContext context) : ISearchRepository
{
    public async Task<(List<HotelSearchItemResponse> Hotels, int TotalCount)> SearchHotelsAsync(
        SearchHotelsCriteria criteria, CancellationToken cancellationToken = default)
    {
        var checkIn = criteria.CheckIn.ToDateTime(TimeOnly.MinValue);
        var checkOut = criteria.CheckOut.ToDateTime(TimeOnly.MinValue);

        var availableRooms = context.Rooms
            .AsNoTracking()
            .Where(r => r.IsAvailable
                        && r.AdultCapacity >= criteria.AdultsPerRoom
                        && r.ChildCapacity >= criteria.ChildrenPerRoom
                        && (criteria.MinPrice == null || r.PricePerNight >= criteria.MinPrice)
                        && (criteria.MaxPrice == null || r.PricePerNight <= criteria.MaxPrice)
                        && !r.Bookings.Any(b =>
                            (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
                            && b.CheckIn < checkOut && b.CheckOut > checkIn));

        var roomStats = availableRooms
            .GroupBy(r => r.HotelId)
            .Select(g => new { HotelId = g.Key, MinPrice = g.Min(r => r.PricePerNight), Count = g.Count() })
            .Where(s => s.Count >= criteria.Rooms);

        var hotels = context.Hotels.AsNoTracking();

        if (criteria.Q is not null)
            hotels = hotels.Where(h => h.Name.Contains(criteria.Q)
                                       || h.City.Name.Contains(criteria.Q)
                                       || h.City.Country.Contains(criteria.Q));
        if (criteria.CityId.HasValue)
            hotels = hotels.Where(h => h.CityId == criteria.CityId);
        if (criteria.Stars.Count > 0)
            hotels = hotels.Where(h => criteria.Stars.Contains(h.StarRating));
        if (criteria.Types.Count > 0)
            hotels = hotels.Where(h => criteria.Types.Contains(h.HotelType));
        if (criteria.Amenities.Count > 0)
            hotels = hotels.Where(h =>
                h.HotelAmenities.Count(ha => criteria.Amenities.Contains(ha.AmenityId)) == criteria.Amenities.Count);

        var query = hotels.Join(roomStats, h => h.HotelId, s => s.HotelId,
            (h, s) => new { Hotel = h, s.MinPrice, s.Count });

        var totalCount = await query.CountAsync(cancellationToken);

        query = criteria.Sort switch
        {
            SearchSorts.PriceAsc => query.OrderBy(x => x.MinPrice).ThenBy(x => x.Hotel.Name),
            SearchSorts.PriceDesc => query.OrderByDescending(x => x.MinPrice).ThenBy(x => x.Hotel.Name),
            SearchSorts.StarsDesc => query.OrderByDescending(x => x.Hotel.StarRating).ThenBy(x => x.MinPrice),
            SearchSorts.StarsAsc => query.OrderBy(x => x.Hotel.StarRating).ThenBy(x => x.MinPrice),
            SearchSorts.Name => query.OrderBy(x => x.Hotel.Name),
            _ => query.OrderByDescending(x => x.Hotel.StarRating)
                .ThenBy(x => x.MinPrice)
                .ThenBy(x => x.Hotel.Name)
        };

        var items = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .Select(x => new HotelSearchItemResponse
            {
                HotelId = x.Hotel.HotelId,
                Name = x.Hotel.Name,
                CityId = x.Hotel.CityId,
                CityName = x.Hotel.City.Name,
                Country = x.Hotel.City.Country,
                StarRating = x.Hotel.StarRating,
                HotelType = x.Hotel.HotelType,
                Address = x.Hotel.Address,
                Description = x.Hotel.Description,
                Latitude = x.Hotel.Latitude,
                Longitude = x.Hotel.Longitude,
                ThumbnailUrl = x.Hotel.Images
                    .Where(i => i.Status == HotelImageStatus.Uploaded && i.Url != null)
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.Url)
                    .FirstOrDefault(),
                MinPricePerNight = x.MinPrice,
                AvailableRooms = x.Count,
                Amenities = x.Hotel.HotelAmenities
                    .Select(ha => ha.Amenity.Name)
                    .OrderBy(name => name)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
