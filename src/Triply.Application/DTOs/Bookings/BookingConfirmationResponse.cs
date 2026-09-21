using System.Text.Json.Serialization;
using Triply.Domain.Enums.Bookings;

namespace Triply.Application.DTOs.Bookings;

public record BookingConfirmationResponse()
{
    /// <summary>Gets the confirmation number.</summary>
    public string ConfirmationNumber { get; init; }
    /// <summary>Gets the booking status.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookingStatus Status { get; init; }
    /// <summary>Gets the guest full name.</summary>
    public string GuestFullName { get; init; }
    /// <summary>Gets the guest email.</summary>
    public string GuestEmail { get; init; }
    /// <summary>Gets the guest phone number.</summary>
    public string? GuestPhoneNumber { get; init; }
    /// <summary>Gets the special requests.</summary>
    public string? SpecialRequests { get; init; }
    /// <summary>Gets the discount amount.</summary>
    public decimal DiscountAmount { get; init; }
    /// <summary>Gets the total price.</summary>
    public decimal TotalPrice { get; init; }
    /// <summary>Gets the booking created date.</summary>
    public DateTime CreatedAt { get; init; }
    /// <summary>Gets the list of booking - read only.</summary>
    public IReadOnlyList<BookingResponse> Bookings { get; init; } = [];
}