using Triply.Domain.Enums.Payments;

namespace Triply.Domain.Entities;

/// <summary>Represents the payment.</summary>
public sealed class Payment
{
    /// <summary>Initializes a new instance of the payment.</summary>
    public Payment()
    {
        PaymentId = Guid.NewGuid();
        Status = PaymentStatus.Pending;
    }
    /// <summary>Gets or sets the identifier of the payment.</summary>
    public Guid PaymentId { get; set; }

    /// <summary>Gets or sets the identifier of the booking.</summary>
    public Guid BookingId { get; set; }

    /// <summary>Gets or sets the provider.</summary>
    public string Provider { get; set; } = null!;

    /// <summary>Gets or sets the amount.</summary>
    public decimal Amount { get; set; }

    /// <summary>Gets or sets the status.</summary>
    public PaymentStatus Status { get; set; }

    /// <summary>Gets or sets the identifier of the transaction.</summary>
    public string? TransactionId { get; set; }

    /// <summary>Gets or sets when the payment was paid.</summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>Gets or sets the navigation property for booking.</summary>
    public Booking Booking { get; set; } = null!;
}