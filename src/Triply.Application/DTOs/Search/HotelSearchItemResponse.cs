using System.Text.Json.Serialization;
using Triply.Domain.Enums.Hotels;

namespace Triply.Application.DTOs.Search;

public record HotelSearchItemResponse
{
    public Guid HotelId { get; init; }
    public string Name { get; init; }
    public Guid CityId { get; init; }
    public string CityName { get; init; }
    public string Country { get; init; }
    public byte StarRating { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HotelType HotelType { get; init; }
    public string? Address { get; init; }
    public string? Description { get; init; }
    public string? ThumbnailUrl { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public decimal MinPricePerNight { get; init; }
    public decimal TotalPrice { get; init; }
    public int AvailableRooms { get; init; }
    public IReadOnlyList<string> Amenities { get; init; } = [];
}
