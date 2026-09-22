using Triply.Application.DTOs.Payments;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the payment operations.</summary>
public interface IPaymentService
{
    /// <summary>Initiates payment for the booking with the given confirmation number.</summary>
    Task<Result<PaymentResponse>> PayAsync(string confirmationNumber, Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>Handles an incoming webhook event from the payment provider.</summary>
    Task<Result<bool>> HandleWebhookAsync(string payload, string? signature,
        CancellationToken cancellationToken = default);
}