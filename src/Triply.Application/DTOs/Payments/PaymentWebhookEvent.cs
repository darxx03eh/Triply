namespace Triply.Application.DTOs.Payments;

/// <summary>Represents a normalized payment event received from a payment provider's webhook.</summary>
public record PaymentWebhookEvent()
{
    /// <summary>Gets the confirmation number of the booking associated with this event.</summary>
    public string ConfirmationNumber { get; init; }
    /// <summary>Gets the payment provider's session identifier associated with this event.</summary>
    public string SessionId { get; init; }
    /// <summary>Gets the payment provider's transaction identifier, if the payment succeeded.</summary>
    public string? TransactionId { get; init; }
    /// <summary>Gets a value indicating whether the event represents a successful payment.</summary>
    public bool IsPaid { get; init; }
    /// <summary>Gets a value indicating whether the event represents a failed payment.</summary>
    public bool IsFailed { get; init; }
}