using System.Text.Json.Serialization;
using Triply.Domain.Enums.Hotels;

namespace Triply.Application.DTOs.Hotels;

public record HotelSummaryResponse()
{
    public Guid HotelId { get; init; }
    public string Name { get; init; }
    public string CityName  { get; init; }
    public Guid? OwnerId { get; init; }
    public string? OwnerName { get; init; }
    public int RoomsCount { get; init; }
    public byte StarRating { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HotelType HotelType { get; init; }
    public string? Address { get; init; }
    public string? ThumbnailUrl { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}