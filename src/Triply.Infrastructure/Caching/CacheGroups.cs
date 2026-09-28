namespace Triply.Infrastructure.Caching;

/// <summary>Defines the cache group names used to group and invalidate related cached values.</summary>
public class CacheGroups
{
    /// <summary>The cache group for cities.</summary>
    public const string Cities = "cities";

    /// <summary>The cache group for hotels.</summary>
    public const string Hotels = "hotels";

    /// <summary>The cache group for amenities.</summary>
    public const string Amenities = "amenities";

    /// <summary>The cache group for rooms.</summary>
    public const string Rooms = "rooms";

    /// <summary>The cache group for bookings.</summary>
    public const string Bookings = "bookings";

    /// <summary>The cache group for deals.</summary>
    public const string Deals = "deals";

    /// <summary>The cache group for reviews.</summary>
    public const string Reviews = "reviews";

    /// <summary>The cache group for attractions.</summary>
    public const string Attractions = "attractions";

    /// <summary>The cache group for hotel images.</summary>
    public const string HotelImages = "hotel-images";

    /// <summary>The cache group for hotel amenities.</summary>
    public const string HotelAmenities = "hotel-amenities";

    /// <summary>The cache group for hotel search results.</summary>
    public const string HotelSearch = "hotel-search";

    /// <summary>The cache group for featured deals.</summary>
    public const string FeaturedDeals = "featured-deals";

    /// <summary>The cache group for trending cities.</summary>
    public const string TrendingCities = "trending-cities";
}