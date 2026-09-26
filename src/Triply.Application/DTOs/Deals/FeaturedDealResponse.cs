using System.Text.Json.Serialization;
using Triply.Domain.Enums.Rooms;

namespace Triply.Application.DTOs.Deals;

/// <summary>Gets or sets the featured deal response.</summary>
/// <summary>Response returned for the featured deal.</summary>
public record FeaturedDealResponse()
{
    /// <summary>Gets the identifier of the deal.</summary>
    public Guid DealId { get; init; }
    /// <summary>Gets the title.</summary>
    public string Title { get; init; }
    /// <summary>Gets the identifier of the hotel.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the hotel name.</summary>
    public string HotelName { get; init; }
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
    /// <summary>Gets the identifier of the room.</summary>
    public Guid RoomId { get; init; }
    /// <summary>Gets the room type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; init; }
    /// <summary>Gets the original price.</summary>
    public decimal OriginalPrice { get; init; }
    /// <summary>Gets the discounted price.</summary>
    public decimal DiscountedPrice { get; init; }
    /// <summary>Gets the discount percentage.</summary>
    public decimal DiscountPercentage { get; init; }
    /// <summary>Gets when the featured deal ends.</summary>
    public DateTime EndsAt { get; init; }
}
