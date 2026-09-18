using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Domain.Enums.Hotels;

namespace Triply.Application.Extensions;

public static class SearchExtensions
{
    public static DateOnly Today() => DateOnly.FromDateTime(DateTime.UtcNow);

    public static DateOnly GetCheckIn(this SearchHotelsRequest request) => request.CheckIn ?? Today();

    public static DateOnly GetCheckOut(this SearchHotelsRequest request)
        => request.CheckOut ?? request.GetCheckIn().AddDays(1);

    public static int GetAdults(this SearchHotelsRequest request) => request.Adults ?? 2;

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
}
