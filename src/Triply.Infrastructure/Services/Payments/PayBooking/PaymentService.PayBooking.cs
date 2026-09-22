using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Payments;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Enums.Payments;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Payments;

/// <summary>Handles initiating payment for a booking.</summary>
public partial class PaymentService
{
    /// <summary>Initiates payment for the bookings under the given confirmation number by creating a checkout session with the payment gateway. If the gateway completes the session immediately (e.g. the mock gateway), the bookings are confirmed right away.</summary>
    /// <param name="confirmationNumber">The confirmation number of the booking(s) to pay for.</param>
    /// <param name="userId">The identifier of the user attempting the payment, used to verify ownership of the booking.</param>
    /// <returns>
    /// A failure with <c>BOOKING_NOT_FOUND</c> if no matching booking exists or it does not belong to <paramref name="userId"/>;
    /// a failure with <c>BOOKING_CANCELLED</c> or <c>BOOKING_ALREADY_PAID</c> if the booking is not in a payable state;
    /// otherwise a success containing the payment session details.
    /// </returns>
    public async Task<Result<PaymentResponse>> PayAsync(string confirmationNumber, Guid userId,
        CancellationToken cancellationToken = default)
    {
        var bookings = await bookingRepository
            .GetByConfirmationNumberAsync(confirmationNumber, cancellationToken);
        if (bookings.Count == 0)
        {
            logger.LogWarning("Pay booking failed: booking {ConfirmationNumber} was not found (user {UserId})",
                confirmationNumber, userId);
            return Result<PaymentResponse>.Failure(
                "BOOKING_NOT_FOUND",
                $"The requested booking with confirmation number: {confirmationNumber} was not found.",
                ResultErrorType.NotFound);
        }

        if (bookings[0].UserId != userId)
        {
            logger.LogWarning(
                "Pay booking failed: booking {ConfirmationNumber} does not belong to user {UserId}",
                confirmationNumber, userId);
            return Result<PaymentResponse>.Failure(
                "BOOKING_NOT_FOUND",
                $"The requested booking with confirmation number: {confirmationNumber} was not found.",
                ResultErrorType.NotFound);
        }

        var status = bookings[0].Status;
        if (status != BookingStatus.Pending)
        {
            logger.LogWarning(
                "Pay booking failed: booking {ConfirmationNumber} is {Status} and can not be paid (user {UserId})",
                confirmationNumber, status, userId);
            return Result<PaymentResponse>.Failure(
                status == BookingStatus.Cancelled ? "BOOKING_CANCELLED" : "BOOKING_ALREADY_PAID",
                $"This booking is {status.ToString().ToLower()} and can not be paid.");
        }

        var session = await paymentGateway.CreateCheckoutSessionAsync(new PaymentSessionRequest
        {
            ConfirmationNumber = confirmationNumber,
            CustomerEmail = bookings[0].GuestEmail,
            Lines = bookings
                .Select(b => new PaymentLine(
                    $"{b.Room.Hotel.Name} - {b.Room.RoomType} room {b.Room.Number} " +
                    $"({b.CheckIn:dd MMM} - {b.CheckOut:dd MMM})", b.TotalPrice))
                .ToList()
        }, cancellationToken);

        foreach (var booking in bookings)
        {
            if (booking.Payment is null)
            {
                var payment = new Payment
                {
                    BookingId = booking.BookingId,
                    Provider = paymentGateway.Name,
                    Amount = booking.TotalPrice,
                    Status = PaymentStatus.Pending,
                    TransactionId = session.SessionId
                };

                // Mark the new dependent explicitly as Added. Assigning a Guid in Payment's
                // constructor otherwise causes EF to issue an UPDATE for a row that does not exist.
                booking.Payment = payment;
                await paymentRepository.AddAsync(payment, cancellationToken);
            }
            else
            {
                booking.Payment.Provider = paymentGateway.Name;
                booking.Payment.Amount = booking.TotalPrice;
                booking.Payment.Status = PaymentStatus.Pending;
                booking.Payment.TransactionId = session.SessionId;
            }
        }
        await bookingRepository.SaveChangesAsync(cancellationToken);

        var amount = bookings.Sum(b => b.TotalPrice);
        logger.LogInformation(
            "Payment session {SessionId} created for booking {ConfirmationNumber} via {Provider} " +
            "(user {UserId}, {RoomCount} rooms, {Amount} {Currency}, completed immediately: {IsCompleted})",
            session.SessionId, confirmationNumber, paymentGateway.Name, userId, bookings.Count,
            amount, paymentOptions.Value.Currency, session.IsCompleted);

        if (session.IsCompleted)
            await ConfirmBookingsAsync(bookings, session.SessionId);

        return Result<PaymentResponse>.Success(new PaymentResponse
            {
                ConfirmationNumber = confirmationNumber,
                Provider = paymentGateway.Name,
                SessionId = session.SessionId,
                CheckoutUrl = session.CheckoutUrl,
                Amount = amount,
                Currency = paymentOptions.Value.Currency,
                BookingStatus = bookings[0].Status
            }, ResultSuccessType.Ok, session.IsCompleted
            ? new ResultSuccess("PAYMENT_COMPLETED", $"Your booking {confirmationNumber} was paid " +
                                                     $"and confirmed.")
            : new ResultSuccess("PAYMENT_STARTED", "Complete the payment using the checkout url."));
    }
}
