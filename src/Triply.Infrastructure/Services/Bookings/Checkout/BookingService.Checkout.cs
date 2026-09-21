using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Bookings;
using Triply.Application.Extensions;
using Triply.Application.Features.Bookings.Commands.Checkout;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Bookings;

public partial class BookingService
{
    /// <summary>Characters used to generate booking confirmation numbers.</summary>
    private const string ConfirmationNumberChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    
    /// <summary>
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<BookingConfirmationResponse>> CheckoutAsync(CheckoutRequest request,
        CancellationToken cancellationToken = default)
    {
        var items = await cartRepository.GetUserCartAsync(request.UserId, cancellationToken);
        if (items.Count == 0)
        {
            logger.LogWarning("Checkout failed: cart of user {UserId} is empty", request.UserId);
            return Result<BookingConfirmationResponse>.Failure(
                "CART_EMPTY", "Your cart is empty, add a room before checkout.");
        }

        var expiredItem = items.FirstOrDefault(i => i.CheckIn < DateTime.UtcNow.Date);
        if (expiredItem is not null)
        {
            logger.LogWarning(
                "Checkout failed: cart item {CartItemId} (room {RoomId}) of user {UserId} " +
                "has a passed check-in date {CheckIn:yyyy-MM-dd}",
                expiredItem.CartItemId, expiredItem.RoomId, request.UserId, expiredItem.CheckIn);
            return Result<BookingConfirmationResponse>.Failure(
                "CART_ITEM_EXPIRED",
                $"The check-in date of room {expiredItem.Room.Number} in {expiredItem.Room.Hotel.Name} has passed, " +
                "remove it from your cart.");
        }

        var discounts = await dealRepository
            .GetActiveDiscountsAsync(items.Select(i => i.RoomId), cancellationToken);
        var confirmationNumber = await GenerateConfirmationNumberAsync(cancellationToken);
        logger.LogDebug(
            "Checkout started for user {UserId} with confirmation number {ConfirmationNumber} ({ItemCount} items," +
            " {DiscountCount} active discounts)",
            request.UserId, confirmationNumber, items.Count, discounts.Count);

        await using var transaction = await bookingRepository.BeginSerializableTransactionAsync(cancellationToken);
        try
        {
            var bookings = new List<Booking>();
            var deletedItems = new List<CartItem>();
            foreach (var item in items)
            {
                var isRoomBooked =
                    await bookingRepository.IsRoomBookedAsync(item.RoomId, item.CheckIn, item.CheckOut,
                        cancellationToken);
                if (!item.Room.IsAvailable || isRoomBooked)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    logger.LogWarning(
                        "Checkout {ConfirmationNumber} failed: room {RoomId} is no longer available for " +
                        "{CheckIn:yyyy-MM-dd} to {CheckOut:yyyy-MM-dd} (room available: {IsAvailable}, " +
                        "already booked: {IsBooked}, user {UserId})",
                        confirmationNumber, item.RoomId, item.CheckIn, item.CheckOut,
                        item.Room.IsAvailable, isRoomBooked, request.UserId);
                    return Result<BookingConfirmationResponse>.Failure(
                        "ROOM_NOT_AVAILABLE",
                        $"Room {item.Room.Number} in {item.Room.Hotel.Name} is no longer available " +
                        "for the selected dates, remove it from your cart.",
                        ResultErrorType.Conflict);
                }

                var price = item.ToCartItemResponse(
                    discounts.TryGetValue(item.RoomId, out var discount) ? discount : null, true);

                var booking = request.ToBooking(confirmationNumber, item, price);

                bookings.Add(booking);
                deletedItems.Add(item);
            }

            await bookingRepository.AddRangeAsync(bookings);
            await cartRepository.DeleteRangeAsync(deletedItems);

            await bookingRepository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation(
                "Booking {ConfirmationNumber} created for user {UserId} ({RoomCount} rooms), cart items removed",
                confirmationNumber, request.UserId, bookings.Count);

            return Result<BookingConfirmationResponse>.Success(
                bookings.ToBookingConfirmationResponse(), ResultSuccessType.Created,
                new ResultSuccess("BOOKING_CREATED",
                    $"Your booking {confirmationNumber} was created, complete the payment to confirm it."));
        }
        catch (Exception exception) when (exception is DbUpdateException or SqlException)
        {
            logger.LogError(exception,
                "Checkout {ConfirmationNumber} failed for user {UserId}: database error while " +
                "saving bookings (possible booking conflict)",
                confirmationNumber, request.UserId);
            return Result<BookingConfirmationResponse>.Failure(
                "BOOKING_CONFLICT",
                "One of the rooms was booked by someone else at the same moment, please try again.",
                ResultErrorType.Conflict);
        }
    }

    private async Task<string> GenerateConfirmationNumberAsync(CancellationToken cancellationToken)
    {
        string confirmationNumber;
        do confirmationNumber = $"TRP-{RandomNumberGenerator.GetString(ConfirmationNumberChars, 8)}";
        while (await bookingRepository.IsConfirmationNumberExistsAsync(confirmationNumber, cancellationToken));

        return confirmationNumber;
    }
}