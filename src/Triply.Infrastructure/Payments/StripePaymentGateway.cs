using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Triply.Application.DTOs.Payments;
using Triply.Application.Interfaces.Payments;
using Triply.Application.Options;

namespace Triply.Infrastructure.Payments;

/// <summary>A payment gateway implementation backed by Stripe Checkout.</summary>
/// <param name="options">The payment configuration, including Stripe credentials and redirect URLs.</param>
public class StripePaymentGateway(IOptions<PaymentOptions> options, ILogger<StripePaymentGateway> logger) 
    : IPaymentGateway
{
    /// <summary>Gets the name of this payment provider.</summary>
    public string Name => "stripe";
    private readonly PaymentOptions _options = options.Value;
    private readonly StripeClient _client = new(options.Value.StripeSecretKey);

    /// <summary>Creates a Stripe Checkout session for the given payment request, converting each line item's amount to the smallest currency unit.</summary>
    public async Task<PaymentSessionResult> CreateCheckoutSessionAsync(PaymentSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var sessionOptions = new SessionCreateOptions
        {
            Mode = "payment",
            CustomerEmail = request.CustomerEmail,
            ClientReferenceId = request.ConfirmationNumber,
            Metadata = new Dictionary<string, string> { ["confirmation_number"] = request.ConfirmationNumber },
            SuccessUrl = _options.SuccessUrl.Replace("{CONFIRMATION_NUMBER}", request.ConfirmationNumber),
            CancelUrl = _options.CancelUrl.Replace("{CONFIRMATION_NUMBER}", request.ConfirmationNumber),
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            LineItems = request.Lines.Select(line => new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = _options.Currency,
                    UnitAmountDecimal = Math.Round(line.Amount * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions { Name = line.Name }
                }
            }).ToList()
        };

        try
        {
            var session = await new SessionService(_client).CreateAsync(sessionOptions,
                cancellationToken: cancellationToken);
            logger.LogInformation(
                "Stripe checkout session {SessionId} created for booking {ConfirmationNumber} " +
                "({LineCount} lines, {Amount} {Currency})",
                session.Id, request.ConfirmationNumber, request.Lines.Count,
                request.Lines.Sum(l => l.Amount), _options.Currency);

            return new PaymentSessionResult
            {
                SessionId = session.Id,
                CheckoutUrl = session.Url,
                IsCompleted = false
            };
        }
        catch (StripeException exception)
        {
            logger.LogError(exception,
                "Stripe checkout session for booking {ConfirmationNumber} could not be created " +
                "(status {StatusCode}, code {StripeCode})",
                request.ConfirmationNumber, exception.HttpStatusCode, exception.StripeError?.Code);
            throw;
        }
    }

    /// <summary>Verifies and parses a raw Stripe webhook payload into a normalized event, handling checkout completion, async payment outcomes, and expiration/failure events.</summary>
    /// <param name="payload">The raw webhook request body.</param>
    /// <param name="signature">The <c>Stripe-Signature</c> header value used to verify the payload's authenticity.</param>
    /// <returns>The parsed event, or <see langword="null"/> if the signature is invalid, the payload is not a checkout session, or the event type is not relevant.</returns>
    public PaymentWebhookEvent? ParseWebhookEvent(string payload, string? signature)
    {
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signature, _options.StripeWebhookSecret,
                throwOnApiVersionMismatch: false);
        }
        catch (StripeException exception)
        {
            logger.LogWarning(
                "Stripe webhook rejected: signature verification or parsing failed " +
                "(signature provided: {HasSignature}, reason: {Reason})",
                signature is not null, exception.Message);
            return null;
        }

        if (stripeEvent.Data.Object is not Session session || session.ClientReferenceId is null)
        {
            logger.LogDebug(
                "Stripe webhook {EventId} ({EventType}) ignored: not a checkout session or has no client reference",
                stripeEvent.Id, stripeEvent.Type);
            return null;
        }

        var paymentEvent = stripeEvent.Type switch
        {
            EventTypes.CheckoutSessionCompleted or EventTypes.CheckoutSessionAsyncPaymentSucceeded
                => new PaymentWebhookEvent
                {
                    ConfirmationNumber = session.ClientReferenceId,
                    SessionId = session.Id,
                    TransactionId = session.PaymentIntentId,
                    IsPaid = session.PaymentStatus == "paid"
                },
            EventTypes.CheckoutSessionExpired or EventTypes.CheckoutSessionAsyncPaymentFailed
                => new PaymentWebhookEvent
                {
                    ConfirmationNumber = session.ClientReferenceId,
                    SessionId = session.Id,
                    IsFailed = true
                },
            _ => null
        };

        if (paymentEvent is null)
        {
            logger.LogDebug("Stripe webhook {EventId} ignored: event type {EventType} is not supported",
                stripeEvent.Id, stripeEvent.Type);
            return null;
        }

        logger.LogInformation(
            "Stripe webhook {EventId} ({EventType}) parsed for booking {ConfirmationNumber} " +
            "(session {SessionId}, paid: {IsPaid}, failed: {IsFailed})",
            stripeEvent.Id, stripeEvent.Type, paymentEvent.ConfirmationNumber, paymentEvent.SessionId,
            paymentEvent.IsPaid, paymentEvent.IsFailed);
        return paymentEvent;
    }

    /// <summary>Refunds a previously captured Stripe payment intent.</summary>
    /// <param name="transactionId">The Stripe payment intent identifier to refund.</param>
    /// <param name="amount">The amount to refund, converted to the smallest currency unit.</param>
    public async Task RefundAsync(string transactionId, decimal amount, CancellationToken cancellationToken = default)
    {
        try
        {
            var refund = await new RefundService(_client).CreateAsync(new RefundCreateOptions
            {
                PaymentIntent = transactionId,
                Amount = (long)Math.Round(amount * 100)
            }, cancellationToken: cancellationToken);
            logger.LogInformation(
                "Stripe refund {RefundId} of {Amount} {Currency} created for transaction " +
                "{TransactionId} (status {Status})",
                refund.Id, amount, _options.Currency, transactionId, refund.Status);
        }
        catch (StripeException exception)
        {
            logger.LogError(exception,
                "Stripe refund of {Amount} {Currency} for transaction {TransactionId} failed " +
                "(status {StatusCode}, code {StripeCode})",
                amount, _options.Currency, transactionId, exception.HttpStatusCode, exception.StripeError?.Code);
            throw;
        }
    }
}