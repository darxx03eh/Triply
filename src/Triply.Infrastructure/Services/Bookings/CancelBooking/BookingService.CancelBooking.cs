using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Bookings;
using Triply.Application.Extensions;
using Triply.Domain.Enums.Bookings;
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
        foreach (var booking in bookings)
            (booking.Status, booking.CanceledAt, booking.ModifiedAt) = (BookingStatus.Cancelled, now, now);
        await bookingRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Booking {ConfirmationNumber} cancelled by user {UserId} (admin: {IsAdmin}, owner {OwnerId}, {RoomCount} rooms)",
            confirmationNumber, userId, isAdmin, bookings[0].UserId, bookings.Count);

        return Result<BookingConfirmationResponse>.Success(
            bookings.ToBookingConfirmationResponse(), ResultSuccessType.Ok,
            new ResultSuccess("BOOKING_CANCELLED", 
                $"Your booking {confirmationNumber} was cancelled successfully."));
    }
}