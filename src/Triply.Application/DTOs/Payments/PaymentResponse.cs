using System.Text.Json.Serialization;
using Triply.Domain.Enums.Bookings;

namespace Triply.Application.DTOs.Payments;

/// <summary>Represents the result of initiating or checking a payment for a booking.</summary>
public record PaymentResponse()
{
    /// <summary>Gets the confirmation number of the booking associated with this payment.</summary>
    public string ConfirmationNumber { get; init; }
    /// <summary>Gets the name of the payment provider that processed this payment (e.g. "Mock", "Stripe").</summary>
    public string Provider { get; init; }
    /// <summary>Gets the payment provider's session identifier for this transaction.</summary>
    public string SessionId { get; init; }
    /// <summary>Gets the URL the client should be redirected to in order to complete checkout, if applicable.</summary>
    public string? CheckoutUrl { get; init; }
    /// <summary>Gets the payment amount.</summary>
    public decimal Amount { get; init; }
    /// <summary>Gets the ISO 4217 currency code of the payment amount.</summary>
    public string Currency { get; init; }
    /// <summary>Gets the current status of the booking associated with this payment.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookingStatus BookingStatus { get; init; }
}