namespace Triply.Domain.Enums.Payments;

/// <summary>The payment status values.</summary>
public enum PaymentStatus : byte
{
    /// <summary>The pending.</summary>
    Pending = 0,
    /// <summary>The paid.</summary>
    Paid = 1,
    /// <summary>The failed.</summary>
    Failed = 2,
    /// <summary>The refunded.</summary>
    Refunded = 3
}