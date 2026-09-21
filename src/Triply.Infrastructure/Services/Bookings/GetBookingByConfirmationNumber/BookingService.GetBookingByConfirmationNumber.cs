using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Bookings;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Bookings;

public partial class BookingService
{
    /// <summary>Gets bookings by confirmation number.</summary>
    
    public async Task<Result<BookingConfirmationResponse>> GetByConfirmationNumberAsync(string confirmationNumber, 
        Guid userId, bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var bookings = await bookingRepository
            .GetByConfirmationNumberAsync(confirmationNumber, cancellationToken);
        if (bookings.Count == 0)
        {
            logger.LogWarning(
                "Get booking failed: booking {ConfirmationNumber} was not found (user {UserId}, admin: {IsAdmin})",
                confirmationNumber, userId, isAdmin);
            return Result<BookingConfirmationResponse>.Failure(
                "BOOKING_NOT_FOUND",
                $"The requested booking with confirmation number: {confirmationNumber} was not found.",
                ResultErrorType.NotFound);
        }

        if (!isAdmin && bookings[0].UserId != userId)
        {
            logger.LogWarning(
                "Get booking failed: booking {ConfirmationNumber} does not belong to user {UserId}",
                confirmationNumber, userId);
            return Result<BookingConfirmationResponse>.Failure(
                "BOOKING_NOT_FOUND",
                $"The requested booking with confirmation number: {confirmationNumber} was not found.",
                ResultErrorType.NotFound);
        }

        logger.LogDebug(
            "Booking {ConfirmationNumber} retrieved by user {UserId} (admin: {IsAdmin}, {RoomCount} rooms)",
            confirmationNumber, userId, isAdmin, bookings.Count);

        return Result<BookingConfirmationResponse>.Success(bookings.ToBookingConfirmationResponse(),
            success: new("BOOKING_FOUND",
                $"The requested booking with confirmation number: {confirmationNumber} was found."));
    }
}