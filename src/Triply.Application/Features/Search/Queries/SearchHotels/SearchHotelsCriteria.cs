using Triply.Domain.Enums.Hotels;

namespace Triply.Application.Features.Search.Queries.SearchHotels;

public sealed record SearchHotelsCriteria
{
    public string? Q { get; init; }
    public Guid? CityId { get; init; }
    public DateOnly CheckIn { get; init; }
    public DateOnly CheckOut { get; init; }
    public int Nights => CheckOut.DayNumber - CheckIn.DayNumber;
    public int Adults { get; init; }
    public int Children { get; init; }
    public int Rooms { get; init; }
    public int AdultsPerRoom => (int)Math.Ceiling(Adults / (double)Rooms);
    public int ChildrenPerRoom => (int)Math.Ceiling(Children / (double)Rooms);
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public IReadOnlyList<byte> Stars { get; init; } = [];
    public IReadOnlyList<HotelType> Types { get; init; } = [];
    public IReadOnlyList<Guid> Amenities { get; init; } = [];
    public string Sort { get; init; } = SearchSorts.Recommended;
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public static class SearchSorts
{
    public const string Recommended = "recommended";
    public const string PriceAsc = "price_asc";
    public const string PriceDesc = "price_desc";
    public const string StarsDesc = "stars_desc";
    public const string StarsAsc = "stars_asc";
    public const string Name = "name";

    public static readonly HashSet<string> All =
        new(StringComparer.OrdinalIgnoreCase) { Recommended, PriceAsc, PriceDesc, StarsDesc, StarsAsc, Name };
}
