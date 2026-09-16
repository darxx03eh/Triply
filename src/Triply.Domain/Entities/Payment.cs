using Triply.Domain.Enums.Payments;

namespace Triply.Domain.Entities;

public sealed class Payment
{
    public Payment()
    {
        PaymentId = Guid.NewGuid();
        Status = PaymentStatus.Pending;
    }
    public Guid PaymentId { get; set; }

    public Guid BookingId { get; set; }

    public string Provider { get; set; } = null!;

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; }

    public string? TransactionId { get; set; }

    public DateTime? PaidAt { get; set; }

    public Booking Booking { get; set; } = null!;
}