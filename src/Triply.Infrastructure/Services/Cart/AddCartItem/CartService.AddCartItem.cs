using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Cart;
using Triply.Application.Extensions;
using Triply.Application.Features.Cart.Commands.AddCartItem;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cart;

public partial class CartService
{
    /// <summary>Add item to cart.</summary>
    public async Task<Result<CartResponse>> AddItemAsync(AddCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null || room.IsDeleted)
        {
            logger.LogWarning("Add cart item failed: room {RoomId} was not found (user {UserId})",
                request.RoomId, request.UserId);
            return Result<CartResponse>.Failure(
                "ROOM_NOT_FOUND",
                $"The requested room with id: {request.RoomId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        if (!room.IsAvailable)
        {
            logger.LogWarning("Add cart item failed: room {RoomId} is not available (user {UserId})",
                request.RoomId, request.UserId);
            return Result<CartResponse>.Failure(
                "ROOM_NOT_AVAILABLE", "This room is not available for booking right now.");
        }

        if (request.Adults > room.AdultCapacity || request.Children > room.ChildCapacity)
        {
            logger.LogWarning(
                "Add cart item failed: room {RoomId} capacity exceeded. Requested {Adults} adults " +
                "and {Children} children, capacity is {AdultCapacity} adults and {ChildCapacity} children (user {UserId})",
                request.RoomId, request.Adults, request.Children,
                room.AdultCapacity, room.ChildCapacity, request.UserId);
            return Result<CartResponse>.Failure(
                "ROOM_CAPACITY_EXCEEDED",
                $"This room fits {room.AdultCapacity} adults and {room.ChildCapacity} children only.");
        }

        var checkIn = request.CheckIn.ToDateTime(TimeOnly.MinValue);
        var checkOut = request.CheckOut.ToDateTime(TimeOnly.MinValue);
        var isRoomBooked = await bookingRepository.IsRoomBookedAsync(room.RoomId, checkIn, checkOut, cancellationToken);
        if (isRoomBooked)
        {
            logger.LogWarning(
                "Add cart item failed: room {RoomId} is already booked for {CheckIn:yyyy-MM-dd} to {CheckOut:yyyy-MM-dd}" +
                " (user {UserId})",
                room.RoomId, checkIn, checkOut, request.UserId);
            return Result<CartResponse>.Failure(
                "ROOM_ALREADY_BOOKED",
                "This room is already booked for the selected dates.",
                ResultErrorType.Conflict);
        }

        await cartRepository.AddAsync(request.ToCartItem(checkIn, checkOut, room.RoomId), cancellationToken);
        await cartRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Room {RoomId} added to cart of user {UserId} ({CheckIn:yyyy-MM-dd} to {CheckOut:yyyy-MM-dd}, " +
            "{Adults} adults, {Children} children)",
            room.RoomId, request.UserId, checkIn, checkOut, request.Adults, request.Children);

        var cart = await GetAsync(request.UserId, cancellationToken);
        return Result<CartResponse>.Success(cart.Value!, ResultSuccessType.Created,
            new ResultSuccess("CART_ITEM_ADDED", "Room added to your cart successfully."));
    }
}