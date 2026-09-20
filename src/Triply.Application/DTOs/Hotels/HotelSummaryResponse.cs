using System.Text.Json.Serialization;
using Triply.Domain.Enums.Hotels;

namespace Triply.Application.DTOs.Hotels;

/// <summary>Gets or sets the hotel summary response.</summary>
/// <summary>Response returned for the hotel summary.</summary>
public record HotelSummaryResponse()
{
    /// <summary>Gets the identifier of the hotel.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the name.</summary>
    public string Name { get; init; }
    /// <summary>Gets the city name.</summary>
    public string CityName  { get; init; }
    /// <summary>Gets the identifier of the owner.</summary>
    public Guid? OwnerId { get; init; }
    /// <summary>Gets the owner name.</summary>
    public string? OwnerName { get; init; }
    /// <summary>Gets the number of rooms.</summary>
    public int RoomsCount { get; init; }
    /// <summary>Gets whether the hotel summary is deleted.</summary>
    public bool IsDeleted { get; init; }
    /// <summary>Gets the star rating.</summary>
    public byte StarRating { get; init; }
    /// <summary>Gets the hotel type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HotelType HotelType { get; init; }
    /// <summary>Gets the address.</summary>
    public string? Address { get; init; }
    /// <summary>Gets the thumbnail URL.</summary>
    public string? ThumbnailUrl { get; init; }
    /// <summary>Gets the latitude.</summary>
    public decimal? Latitude { get; init; }
    /// <summary>Gets the longitude.</summary>
    public decimal? Longitude { get; init; }
    /// <summary>Gets when the hotel summary was created.</summary>
    public DateTime CreatedAt { get; init; }
    /// <summary>Gets when the hotel summary was modified.</summary>
    public DateTime? ModifiedAt { get; init; }
}