using Triply.Domain.Enums.Hotels;

namespace Triply.Application.Features.Search.Queries.SearchHotels;

/// <summary>Represents the search hotels criteria.</summary>
public sealed record SearchHotelsCriteria
{
    /// <summary>Gets the q.</summary>
    public string? Q { get; init; }
    /// <summary>Gets the identifier of the city.</summary>
    public Guid? CityId { get; init; }
    /// <summary>Gets the check in.</summary>
    public DateOnly CheckIn { get; init; }
    /// <summary>Gets the check out.</summary>
    public DateOnly CheckOut { get; init; }
    /// <summary>Gets the nights.</summary>
    public int Nights => CheckOut.DayNumber - CheckIn.DayNumber;
    /// <summary>Gets the adults.</summary>
    public int Adults { get; init; }
    /// <summary>Gets the children.</summary>
    public int Children { get; init; }
    /// <summary>Gets the rooms.</summary>
    public int Rooms { get; init; }
    /// <summary>Gets the adults per room.</summary>
    public int AdultsPerRoom => (int)Math.Ceiling(Adults / (double)Rooms);
    /// <summary>Gets the children per room.</summary>
    public int ChildrenPerRoom => (int)Math.Ceiling(Children / (double)Rooms);
    /// <summary>Gets the min price.</summary>
    public decimal? MinPrice { get; init; }
    /// <summary>Gets the max price.</summary>
    public decimal? MaxPrice { get; init; }
    /// <summary>Gets the stars.</summary>
    public IReadOnlyList<byte> Stars { get; init; } = [];
    /// <summary>Gets the types.</summary>
    public IReadOnlyList<HotelType> Types { get; init; } = [];
    /// <summary>Gets the amenities.</summary>
    public IReadOnlyList<Guid> Amenities { get; init; } = [];
    /// <summary>Gets the sort.</summary>
    public string Sort { get; init; } = SearchSorts.Recommended;
    /// <summary>Gets the page.</summary>
    public int Page { get; init; }
    /// <summary>Gets the page size.</summary>
    public int PageSize { get; init; }
}

/// <summary>Represents the search sorts.</summary>
public static class SearchSorts
{
    /// <summary>The recommended.</summary>
    public const string Recommended = "recommended";
    /// <summary>The price asc.</summary>
    public const string PriceAsc = "price_asc";
    /// <summary>The price desc.</summary>
    public const string PriceDesc = "price_desc";
    /// <summary>The stars desc.</summary>
    public const string StarsDesc = "stars_desc";
    /// <summary>The stars asc.</summary>
    public const string StarsAsc = "stars_asc";
    /// <summary>The name.</summary>
    public const string Name = "name";

    /// <summary>The all.</summary>
    public static readonly HashSet<string> All =
        new(StringComparer.OrdinalIgnoreCase) { Recommended, PriceAsc, PriceDesc, StarsDesc, StarsAsc, Name };
}
