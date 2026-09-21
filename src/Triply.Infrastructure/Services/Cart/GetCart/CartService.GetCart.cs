using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Cart;
using Triply.Application.Extensions;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Cart;

public partial class CartService
{
    /// <summary>Gets user's cart.</summary>
    public async Task<Result<CartResponse>> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await cartRepository.GetUserCartAsync(userId, cancellationToken);
        var discounts = await dealRepository
            .GetActiveDiscountsAsync(items.Select(i => i.RoomId), cancellationToken);

        var responses = new List<CartItemResponse>();
        foreach (var item in items)
        {
            var isAvailable = item.Room.IsAvailable
                              && !await bookingRepository
                                  .IsRoomBookedAsync(item.RoomId, item.CheckIn, item.CheckOut,
                                  cancellationToken);
            responses.Add(item.ToCartItemResponse(
                discounts.TryGetValue(item.RoomId, out var discount) ? discount : null, isAvailable));
        }

        logger.LogDebug("Cart of user {UserId} retrieved with {ItemCount} items and {DiscountCount} active discounts",
            userId, items.Count, discounts.Count);

        return Result<CartResponse>.Success(responses.ToCartResponse(), success: items.Count == 0
            ? new("CART_EMPTY", "Your cart is empty.")
            : new("CART_FOUND", "Your cart was retrieved successfully."));
    }
}