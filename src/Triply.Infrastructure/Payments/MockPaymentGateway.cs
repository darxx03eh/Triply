using System.Text.Json;
using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Payments;
using Triply.Application.Interfaces.Payments;

namespace Triply.Infrastructure.Payments;

/// <summary>A no-op payment gateway used for local development and testing. Treats every checkout as immediately completed and performs no real payment processing.</summary>
public class MockPaymentGateway(ILogger<MockPaymentGateway> logger) : IPaymentGateway
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>Gets the name of this payment provider.</summary>
    public string Name => "mock";

    /// <summary>Creates a mock checkout session that is marked as completed immediately, without redirecting to any external checkout page.</summary>
    public Task<PaymentSessionResult> CreateCheckoutSessionAsync(PaymentSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var sessionId = $"mock_{Guid.NewGuid():N}";
        logger.LogInformation(
            "Mock checkout session {SessionId} created for booking {ConfirmationNumber} and completed immediately, " +
            "no real payment was processed",
            sessionId, request.ConfirmationNumber);

        return Task.FromResult(new PaymentSessionResult
        {
            SessionId = sessionId,
            CheckoutUrl = null,
            IsCompleted = true
        });
    }

    /// <summary>Deserializes the raw payload directly into a <see cref="PaymentWebhookEvent"/>, ignoring the signature since there is no real provider to verify against.</summary>
    /// <param name="payload">The raw webhook request body, expected to be JSON matching <see cref="PaymentWebhookEvent"/>.</param>
    /// <param name="signature">Unused; present only to satisfy the <see cref="IPaymentGateway"/> contract.</param>
    /// <returns>The deserialized event, or <see langword="null"/> if the payload is not valid JSON for <see cref="PaymentWebhookEvent"/>.</returns>
    public PaymentWebhookEvent? ParseWebhookEvent(string payload, string? signature)
    {
        try
        {
            var paymentEvent = JsonSerializer.Deserialize<PaymentWebhookEvent>(payload, JsonOptions);
            if (paymentEvent is null)
            {
                logger.LogWarning("Mock payment webhook rejected: payload deserialized to null");
                return null;
            }

            logger.LogDebug(
                "Mock payment webhook parsed for booking {ConfirmationNumber} (session {SessionId}, " +
                "paid: {IsPaid}, failed: {IsFailed})",
                paymentEvent.ConfirmationNumber, paymentEvent.SessionId, paymentEvent.IsPaid,
                paymentEvent.IsFailed);
            return paymentEvent;
        }
        catch (JsonException exception)
        {
            logger.LogWarning("Mock payment webhook rejected: payload is not valid JSON ({Reason})",
                exception.Message);
            return null;
        }
    }

    /// <summary>Does nothing, since the mock gateway never captures real funds to refund.</summary>
    /// <param name="transactionId">Unused; present only to satisfy the <see cref="IPaymentGateway"/> contract.</param>
    /// <param name="amount">Unused; present only to satisfy the <see cref="IPaymentGateway"/> contract.</param>
    public Task RefundAsync(string transactionId, decimal amount, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Mock refund of {Amount} for transaction {TransactionId} skipped, no real funds were captured",
            amount, transactionId);
        return Task.CompletedTask;
    }
}