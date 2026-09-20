using System.Text.Json.Serialization;
using Triply.Domain.Enums.Rooms;

namespace Triply.Application.DTOs.Cart;

/// <summary>Public representation of a cart item.</summary>
public record CartItemResponse
{
    /// <summary>Gets the identifier of the cart item.</summary>
    public Guid CartItemId { get; init; }
    /// <summary>Gets the identifier of the room.</summary>
    public Guid RoomId { get; init; }
    /// <summary>Gets the room number.</summary>
    public string RoomNumber { get; init; }
    /// <summary>Gets the room type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; init; }
    /// <summary>Gets the identifier of the hotel.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the hotel name.</summary>
    public string HotelName { get; init; }
    /// <summary>Gets the city name.</summary>
    public string CityName { get; init; }
    /// <summary>Gets the thumbnail url.</summary>
    public string? ThumbnailUrl { get; init; }
    /// <summary>Gets the check in.</summary>
    public DateOnly CheckIn { get; init; }
    /// <summary>Gets the check-out.</summary>
    public DateOnly CheckOut { get; init; }
    /// <summary>Gets the nights.</summary>
    public int Nights { get; init; }
    /// <summary>Gets the adults.</summary>
    public short Adults { get; init; }
    /// <summary>Gets the children.</summary>
    public short Children { get; init; }
    /// <summary>Gets the price per night.</summary>
    public decimal PricePerNight { get; init; }
    /// <summary>Gets the discount percentage.</summary>
    public decimal? DiscountPercentage { get; init; }
    /// <summary>Gets the original price.</summary>
    public decimal OriginalPrice { get; init; }
    /// <summary>Gets the discount amount.</summary>
    public decimal DiscountAmount { get; init; }
    /// <summary>Gets the total price.</summary>
    public decimal TotalPrice { get; init; }
    /// <summary>Gets the IsAvailable.</summary>
    public bool IsAvailable { get; init; }
}