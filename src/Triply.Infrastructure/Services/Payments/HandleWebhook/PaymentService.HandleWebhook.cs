using Microsoft.Extensions.Logging;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Enums.Payments;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Payments;

/// <summary>Handles incoming webhook events from the payment gateway.</summary>
public partial class PaymentService
{
    /// <summary>Parses and applies an incoming payment gateway webhook event: confirms the associated bookings if the payment succeeded, marks their payment as failed if it failed, or ignores the event if it does not match a pending booking's current session.</summary>
    /// <param name="payload">The raw webhook request body.</param>
    /// <param name="signature">The signature header used by the gateway to verify the payload's authenticity, if provided.</param>
    /// <returns>
    /// A failure with <c>PAYMENT_WEBHOOK_INVALID</c> if the payload could not be parsed or verified;
    /// a failure with <c>BOOKING_NOT_FOUND</c> if no booking matches the event's confirmation number;
    /// a success with <c>PAYMENT_WEBHOOK_IGNORED</c> if the booking is not pending or the event's session does not match its current payment;
    /// otherwise a success with <c>PAYMENT_WEBHOOK_HANDLED</c> once the event has been applied.
    /// </returns>
    public async Task<Result<bool>> HandleWebhookAsync(string payload, string? signature,
        CancellationToken cancellationToken = default)
    {
        var paymentEvent = paymentGateway.ParseWebhookEvent(payload, signature);
        if (paymentEvent is null)
        {
            logger.LogWarning(
                "Payment webhook rejected: event from {Provider} is invalid or not supported " +
                "(signature provided: {HasSignature})",
                paymentGateway.Name, signature is not null);
            return Result<bool>.Failure(
                "PAYMENT_WEBHOOK_INVALID", 
                "The payment event is invalid or not supported.");
        }

        var bookings = await bookingRepository.GetByConfirmationNumberAsync(
            paymentEvent.ConfirmationNumber, cancellationToken);
        if (bookings.Count == 0)
        {
            logger.LogWarning(
                "Payment webhook failed: booking {ConfirmationNumber} was not found " +
                "(session {SessionId}, transaction {TransactionId}, paid: {IsPaid})",
                paymentEvent.ConfirmationNumber, paymentEvent.SessionId, paymentEvent.TransactionId,
                paymentEvent.IsPaid);
            return Result<bool>.Failure(
                "BOOKING_NOT_FOUND",
                $"The requested booking with confirmation number: {paymentEvent.ConfirmationNumber} was not found.",
                ResultErrorType.NotFound);
        }

        var status = bookings[0].Status;
        if (status == BookingStatus.Cancelled && paymentEvent.IsPaid)
            logger.LogWarning(
                "Booking {ConfirmationNumber} was paid after it was cancelled, refund it manually " +
                "(session {SessionId}, transaction {TransactionId})",
                paymentEvent.ConfirmationNumber, paymentEvent.SessionId, paymentEvent.TransactionId);

        var sessionMatches = bookings.All(b => b.Payment?.TransactionId == paymentEvent.SessionId);
        if (status != BookingStatus.Pending || !sessionMatches)
        {
            logger.LogInformation(
                "Payment webhook for booking {ConfirmationNumber} ignored: booking status is {Status}, " +
                "session {SessionId} matches current payment: {SessionMatches}",
                paymentEvent.ConfirmationNumber, status, paymentEvent.SessionId, sessionMatches);
            return Result<bool>.Success(true, success: 
                new("PAYMENT_WEBHOOK_IGNORED", "Payment event ignored."));
        }

        if (paymentEvent.IsPaid)
            await ConfirmBookingsAsync(bookings, paymentEvent.TransactionId);
        else if (paymentEvent.IsFailed)
        {
            foreach (var booking in bookings)
                booking.Payment!.Status = PaymentStatus.Failed;
            await bookingRepository.SaveChangesAsync(cancellationToken);
            logger.LogWarning(
                "Payment of booking {ConfirmationNumber} failed (session {SessionId}, {RoomCount} rooms)",
                paymentEvent.ConfirmationNumber, paymentEvent.SessionId, bookings.Count);
        }
        else
            logger.LogDebug(
                "Payment webhook for booking {ConfirmationNumber} (session {SessionId}) is neither paid nor failed, nothing to apply",
                paymentEvent.ConfirmationNumber, paymentEvent.SessionId);

        return Result<bool>.Success(true, success: new
            ("PAYMENT_WEBHOOK_HANDLED", "Payment event handled."));
    }
}