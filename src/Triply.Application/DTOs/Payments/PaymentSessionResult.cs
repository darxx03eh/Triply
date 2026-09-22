namespace Triply.Application.DTOs.Payments;

/// <summary>Represents the outcome of creating a payment session with the payment provider.</summary>
public record PaymentSessionResult
{
    /// <summary>Gets the payment provider's session identifier.</summary>
    public string SessionId { get; init; }
    /// <summary>Gets the URL the client should be redirected to in order to complete checkout, if applicable.</summary>
    public string? CheckoutUrl { get; init; }
    /// <summary>Gets a value indicating whether the payment session was already completed at creation time (e.g. for zero-amount bookings).</summary>
    public bool IsCompleted { get; init; }
}