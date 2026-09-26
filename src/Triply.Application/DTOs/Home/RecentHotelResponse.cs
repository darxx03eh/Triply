namespace Triply.Application.DTOs.Home;

/// <summary>Gets or sets the recent hotel response.</summary>
/// <summary>Response returned for the recent hotel.</summary>
public record RecentHotelResponse()
{
    /// <summary>Gets the identifier of the hotel.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the name.</summary>
    public string Name { get; init; }
    /// <summary>Gets the city name.</summary>
    public string CityName { get; init; }
    /// <summary>Gets the country.</summary>
    public string Country { get; init; }
    /// <summary>Gets the star rating.</summary>
    public byte StarRating { get; init; }
    /// <summary>Gets the average rating submitted by guests.</summary>
    public decimal? AverageRating { get; init; }
    /// <summary>Gets the number of guest reviews.</summary>
    public int ReviewsCount { get; init; }
    /// <summary>Gets the thumbnail URL.</summary>
    public string? ThumbnailUrl { get; init; }
    /// <summary>Gets the min price per night.</summary>
    public decimal? MinPricePerNight { get; init; }
    /// <summary>Gets when the recent hotel was visited.</summary>
    public DateTime VisitedAt { get; init; }
}
