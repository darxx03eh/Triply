using System.Text.Json.Serialization;
using Triply.Domain.Enums.Hotels;

namespace Triply.Application.DTOs.Hotels;

/// <summary>Gets or sets the hotel response.</summary>
/// <summary>Response returned for the hotel.</summary>
public record HotelResponse()
{
    /// <summary>Gets the identifier of the hotel.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the name.</summary>
    public string Name { get; init; }
    /// <summary>Gets the identifier of the city.</summary>
    public Guid CityId { get; init; }
    /// <summary>Gets the city name.</summary>
    public string CityName { get; init; }
    /// <summary>Gets the identifier of the owner.</summary>
    public Guid? OwnerId { get; init; }
    /// <summary>Gets the star rating.</summary>
    public byte StarRating { get; init; }
    /// <summary>Gets the hotel type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HotelType HotelType { get; init; }
    /// <summary>Gets the address.</summary>
    public string? Address { get; init; }
    /// <summary>Gets the description.</summary>
    public string Description { get; init; }
    /// <summary>Gets the latitude.</summary>
    public decimal? Latitude { get; init; }
    /// <summary>Gets the longitude.</summary>
    public decimal? Longitude { get; init; }
    /// <summary>Gets the image urls.</summary>
    public IReadOnlyList<string> ImageUrls { get; init; }
    /// <summary>Gets the amenities.</summary>
    public IReadOnlyList<string> Amenities { get; init; } = [];
    /// <summary>Gets the average rating.</summary>
    public decimal? AverageRating { get; init; }
    /// <summary>Gets the number of reviews.</summary>
    public int ReviewsCount { get; init; }
    /// <summary>Gets when the hotel was created.</summary>
    public DateTime CreatedAt { get; init; }
    /// <summary>Gets when the hotel was modified.</summary>
    public DateTime? ModifiedAt { get; init; }
    /// <summary>Concurrency token (base64 in JSON); send it back unchanged when updating the hotel.</summary>
    public byte[] RowVersion { get; init; } = [];
}