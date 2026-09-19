using System.Text.Json.Serialization;
using Triply.Domain.Enums.Rooms;

namespace Triply.Application.DTOs.Deals;

public record FeaturedDealResponse()
{
    public Guid DealId { get; init; }
    public string Title { get; init; }
    public Guid HotelId { get; init; }
    public string HotelName { get; init; }
    public string CityName { get; init; }
    public string Country { get; init; }
    public byte StarRating { get; init; }
    public string? ThumbnailUrl { get; init; }
    public Guid RoomId { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; init; }
    public decimal OriginalPrice { get; init; }
    public decimal DiscountedPrice { get; init; }
    public decimal DiscountPercentage { get; init; }
    public DateTime EndsAt { get; init; }
}