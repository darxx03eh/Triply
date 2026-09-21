using System.Text.Json.Serialization;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Enums.Rooms;

namespace Triply.Application.DTOs.Bookings;

public record BookingResponse()
{
    /// <summary>Gets the booking identifier.</summary>
    public Guid BookingId { get; init; }
    /// <summary>Gets the confirmation number.</summary>
    public string ConfirmationNumber { get; init; }
    /// <summary>Gets the hotel identifier.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the hotel name.</summary>
    public string HotelName { get; init; }
    /// <summary>Gets the city name.</summary>
    public string CityName { get; init; }
    /// <summary>Gets the hotel address.</summary>
    public string? HotelAddress { get; init; }
    /// <summary>Gets the room identifier.</summary>
    public Guid RoomId { get; init; }
    /// <summary>Gets the room number.</summary>
    public string RoomNumber { get; init; }
    /// <summary>Gets the room type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; init; }
    /// <summary>Gets the check in date.</summary>
    public DateOnly CheckIn { get; init; }
    /// <summary>Gets the check-out date.</summary>
    public DateOnly CheckOut { get; init; }
    /// <summary>Gets the nights number.</summary>
    public int Nights { get; init; }
    /// <summary>Gets the adults number.</summary>
    public short Adults { get; init; }
    /// <summary>Gets the children number.</summary>
    public short Children { get; init; }
    /// <summary>Gets the discount amount.</summary>
    public decimal DiscountAmount { get; init; }
    /// <summary>Gets the total price.</summary>
    public decimal TotalPrice { get; init; }
    /// <summary>Gets the booking status.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookingStatus Status { get; init; }
    /// <summary>Gets the booking created date.</summary>
    public DateTime CreatedAt { get; init; }
}