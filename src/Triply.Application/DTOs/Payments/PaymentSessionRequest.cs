namespace Triply.Application.DTOs.Payments;

/// <summary>Represents a request to create a new payment session for a booking.</summary>
public record PaymentSessionRequest
{
    /// <summary>Gets the confirmation number of the booking this payment session is for.</summary>
    public string ConfirmationNumber { get; init; }
    /// <summary>Gets the customer's email address, used by the payment provider for receipts.</summary>
    public string CustomerEmail { get; init; }
    /// <summary>Gets the line items to include in the payment session.</summary>
    public IReadOnlyList<PaymentLine> Lines { get; init; } = [];
}

/// <summary>Represents a single line item within a payment session.</summary>
/// <param name="Name">The display name of the line item.</param>
/// <param name="Amount">The charge amount for the line item.</param>
public record PaymentLine(string Name, decimal Amount);