using Triply.Application.DTOs.Search;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Domain.Enums.Hotels;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for search.</summary>
public static class SearchExtensions
{
    /// <summary>Maps the search to a day.</summary>
    public static DateOnly Today() => DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>Gets the check in.</summary>
    public static DateOnly GetCheckIn(this SearchHotelsRequest request) => request.CheckIn ?? Today();

    /// <summary>Gets the check out.</summary>
    public static DateOnly GetCheckOut(this SearchHotelsRequest request)
        => request.CheckOut ?? request.GetCheckIn().AddDays(1);

    /// <summary>Gets the adults.</summary>
    public static int GetAdults(this SearchHotelsRequest request) => request.Adults ?? 2;

    /// <summary>Maps the search to a criteria.</summary>
    public static SearchHotelsCriteria ToCriteria(this SearchHotelsRequest request)
        => new SearchHotelsCriteria
        {
            Q = string.IsNullOrWhiteSpace(request.Q) ? null : request.Q.Trim(),
            CityId = request.CityId,
            CheckIn = request.GetCheckIn(),
            CheckOut = request.GetCheckOut(),
            Adults = request.GetAdults(),
            Children = request.Children ?? 0,
            Rooms = request.Rooms ?? 1,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            Stars = (request.Stars ?? []).Distinct().Select(star => (byte)star).ToList(),
            Types = (request.Types ?? [])
                .Select(type => Enum.Parse<HotelType>(type, ignoreCase: true))
                .Distinct()
                .ToList(),
            Amenities = (request.Amenities ?? []).Distinct().ToList(),
            Sort = string.IsNullOrWhiteSpace(request.Sort) ? SearchSorts.Recommended : request.Sort.ToLowerInvariant(),
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10
        };

    /// <summary>Maps the criteria to search response.</summary>
    public static SearchHotelsResponse ToSearchHotelsResponse(this SearchHotelsCriteria criteria,
        IList<HotelSearchItemResponse> hotels, int totalCount)
        => new SearchHotelsResponse()
        {
            Items = hotels.Select(h => h 
                with
                {
                    TotalPrice = h.MinPricePerNight * criteria.Nights * criteria.Rooms
                }).ToList(),
            Page = criteria.Page,
            PageSize = criteria.PageSize,
            TotalCount = totalCount,
            CheckIn = criteria.CheckIn,
            CheckOut = criteria.CheckOut,
            Nights = criteria.Nights,
            Adults = criteria.Adults,
            Children = criteria.Children,
            Rooms = criteria.Rooms
        };
}
