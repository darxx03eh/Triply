using System.Text.Json.Serialization;
using Triply.Domain.Enums.Hotels;

namespace Triply.Application.DTOs.Search;

/// <summary>Response returned for the hotel search item.</summary>
public record HotelSearchItemResponse
{
    /// <summary>Gets the identifier of the hotel.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the name.</summary>
    public string Name { get; init; }
    /// <summary>Gets the identifier of the city.</summary>
    public Guid CityId { get; init; }
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
    /// <summary>Gets the hotel type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HotelType HotelType { get; init; }
    /// <summary>Gets the address.</summary>
    public string? Address { get; init; }
    /// <summary>Gets the description.</summary>
    public string? Description { get; init; }
    /// <summary>Gets the thumbnail URL.</summary>
    public string? ThumbnailUrl { get; init; }
    /// <summary>Gets the latitude.</summary>
    public decimal? Latitude { get; init; }
    /// <summary>Gets the longitude.</summary>
    public decimal? Longitude { get; init; }
    /// <summary>Gets the min price per night.</summary>
    public decimal MinPricePerNight { get; init; }
    /// <summary>Gets the total price.</summary>
    public decimal TotalPrice { get; init; }
    /// <summary>Gets the available rooms.</summary>
    public int AvailableRooms { get; init; }
    /// <summary>Gets the amenities.</summary>
    public IReadOnlyList<string> Amenities { get; init; } = [];
}
