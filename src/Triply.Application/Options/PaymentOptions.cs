namespace Triply.Application.Options;

/// <summary>Configuration options for payment.</summary>
public class PaymentOptions
{
    /// <summary>Gets or sets the payment provider to use (e.g. "Mock", "Stripe").</summary>
    public string Provider { get; set; } = "Mock";
    /// <summary>Gets or sets the ISO 4217 currency code used for payments.</summary>
    public string Currency { get; set; } = "usd";
    /// <summary>Gets or sets the URL the customer is redirected to after a successful payment.</summary>
    public string SuccessUrl { get; set; } = "http://localhost:3000/booking/{CONFIRMATION_NUMBER}?payment=success";
    /// <summary>Gets or sets the URL the customer is redirected to after a cancelled payment.</summary>
    public string CancelUrl { get; set; } = "http://localhost:3000/booking/{CONFIRMATION_NUMBER}?payment=cancelled";
    /// <summary>Gets or sets the Stripe secret API key used to authenticate server-side requests.</summary>
    public string? StripeSecretKey { get; set; }
    /// <summary>Gets or sets the secret used to verify incoming Stripe webhook signatures.</summary>
    public string? StripeWebhookSecret { get; set; }
}
