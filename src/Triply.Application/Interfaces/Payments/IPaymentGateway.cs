using Triply.Application.DTOs.Payments;

namespace Triply.Application.Interfaces.Payments;

/// <summary>Defines the operations a payment provider integration must implement.</summary>
public interface IPaymentGateway
{
    /// <summary>Gets the name of the payment provider (e.g. "Mock", "Stripe").</summary>
    string Name { get; }

    /// <summary>Creates a checkout session with the payment provider for the given request.</summary>
    Task<PaymentSessionResult> CreateCheckoutSessionAsync(PaymentSessionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Parses a raw webhook payload from the payment provider into a normalized event.</summary>
    /// <param name="payload">The raw webhook request body.</param>
    /// <param name="signature">The signature header used to verify the payload's authenticity, if provided.</param>
    /// <returns>The parsed event, or <see langword="null"/> if the payload could not be verified or parsed.</returns>
    PaymentWebhookEvent? ParseWebhookEvent(string payload, string? signature);

    /// <summary>Refunds a previously captured transaction.</summary>
    /// <param name="transactionId">The payment provider's transaction identifier to refund.</param>
    /// <param name="amount">The amount to refund.</param>
    Task RefundAsync(string transactionId, decimal amount, CancellationToken cancellationToken = default);
}