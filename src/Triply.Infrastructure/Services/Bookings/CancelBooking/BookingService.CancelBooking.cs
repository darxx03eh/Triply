using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Bookings;
using Triply.Application.Extensions;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Enums.Payments;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Bookings;

public partial class BookingService
{
    /// <summary>Cancel booking.</summary>
    public async Task<Result<BookingConfirmationResponse>> CancelAsync(string confirmationNumber, Guid userId, 
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var bookings = await bookingRepository
            .GetByConfirmationNumberAsync(confirmationNumber, cancellationToken);
        if (bookings.Count == 0)
        {
            logger.LogWarning(
                "Cancel booking failed: booking {ConfirmationNumber} was not found (user {UserId}, admin: {IsAdmin})",
                confirmationNumber, userId, isAdmin);
            return Result<BookingConfirmationResponse>.Failure(
                "BOOKING_NOT_FOUND",
                $"The requested booking with confirmation number: {confirmationNumber} was not found.",
                ResultErrorType.NotFound);
        }

        if (!isAdmin && bookings[0].UserId != userId)
        {
            logger.LogWarning(
                "Cancel booking failed: booking {ConfirmationNumber} does not belong to user {UserId}",
                confirmationNumber, userId);
            return Result<BookingConfirmationResponse>.Failure(
                "BOOKING_NOT_FOUND",
                $"The requested booking with confirmation number: {confirmationNumber} was not found.",
                ResultErrorType.NotFound);
        }

        var status = bookings[0].Status;
        if (status is BookingStatus.Cancelled or BookingStatus.Completed)
        {
            logger.LogWarning(
                "Cancel booking failed: booking {ConfirmationNumber} is already {Status} (user {UserId})",
                confirmationNumber, status, userId);
            return Result<BookingConfirmationResponse>.Failure(
                "BOOKING_CANNOT_BE_CANCELLED",
                $"This booking is already {status.ToString().ToLower()}.");
        }

        var earliestCheckIn = bookings.Min(b => b.CheckIn);
        if (earliestCheckIn <= DateTime.UtcNow.Date)
        {
            logger.LogWarning(
                "Cancel booking failed: booking {ConfirmationNumber} already started (check-in {CheckIn:yyyy-MM-dd}," +
                " user {UserId})",
                confirmationNumber, earliestCheckIn, userId);
            return Result<BookingConfirmationResponse>.Failure(
                "BOOKING_ALREADY_STARTED",
                "A booking can not be cancelled on or after the check-in date.");
        }

        var now = DateTime.UtcNow;
        var refundedCount = 0;
        foreach (var booking in bookings)
        {
            (booking.Status, booking.CanceledAt, booking.ModifiedAt) = (BookingStatus.Cancelled, now, now);
            if (booking.Payment is
                {
                    Status: PaymentStatus.Paid,
                    TransactionId: not null
                } payment
                && payment.Provider == paymentGateway.Name)
            {
                try
                {
                    await paymentGateway.RefundAsync(payment.TransactionId, payment.Amount, cancellationToken);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception,
                        "Cancel booking {ConfirmationNumber} failed: refund of {Amount} for booking {BookingId} " +
                        "via {Provider} (transaction {TransactionId}) could not be created, {RefundedCount} of " +
                        "{RoomCount} refunds were already sent to the gateway before this one",
                        confirmationNumber, payment.Amount, booking.BookingId, payment.Provider,
                        payment.TransactionId, refundedCount, bookings.Count);
                    throw;
                }

                payment.Status = PaymentStatus.Refunded;
                refundedCount++;
                logger.LogInformation(
                    "Payment of booking {BookingId} under {ConfirmationNumber} refunded: {Amount} via {Provider} " +
                    "(transaction {TransactionId})",
                    booking.BookingId, confirmationNumber, payment.Amount, payment.Provider, payment.TransactionId);
            }
            else if (booking.Payment is { Status: PaymentStatus.Paid } paidPayment)
            {
                logger.LogWarning(
                    "Booking {BookingId} under {ConfirmationNumber} was cancelled but its paid payment was not " +
                    "refunded automatically (payment provider {PaymentProvider}, current gateway {Provider}, " +
                    "has transaction id: {HasTransactionId}), refund it manually",
                    booking.BookingId, confirmationNumber, paidPayment.Provider, paymentGateway.Name,
                    paidPayment.TransactionId is not null);
            }
        }

        await bookingRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Booking {ConfirmationNumber} cancelled by user {UserId} (admin: {IsAdmin}, owner {OwnerId}, " +
            "{RoomCount} rooms, {RefundedCount} refunded)",
            confirmationNumber, userId, isAdmin, bookings[0].UserId, bookings.Count, refundedCount);

        return Result<BookingConfirmationResponse>.Success(
            bookings.ToBookingConfirmationResponse(), ResultSuccessType.Ok,
            new ResultSuccess("BOOKING_CANCELLED", status == BookingStatus.Confirmed
                ? $"Your booking {confirmationNumber} was cancelled and the payment was refunded."
                : $"Your booking {confirmationNumber} was cancelled successfully."));
    }
}