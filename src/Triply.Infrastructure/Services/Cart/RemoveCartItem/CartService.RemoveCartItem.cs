using Microsoft.Extensions.Logging;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cart;

public partial class CartService
{
    /// <summary>Remove item from cart.</summary>
    public async Task<Result<bool>> RemoveItemAsync(Guid cartItemId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        var item = await cartRepository.GetByIdAsync(cartItemId, cancellationToken);
        if (item is null)
        {
            logger.LogWarning("Remove cart item failed: cart item {CartItemId} was not found (user {UserId})",
                cartItemId, userId);
            return Result<bool>.Failure(
                "CART_ITEM_NOT_FOUND",
                $"The requested cart item with id: {cartItemId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        if (item.UserId != userId)
        {
            logger.LogWarning(
                "Remove cart item failed: cart item {CartItemId} does not belong to user {UserId}",
                cartItemId, userId);
            return Result<bool>.Failure(
                "CART_ITEM_NOT_FOUND",
                $"The requested cart item with id: {cartItemId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        await cartRepository.DeleteAsync(item);
        await cartRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Cart item {CartItemId} (room {RoomId}) removed from cart of user {UserId}",
            cartItemId, item.RoomId, userId);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ("CART_ITEM_REMOVED", "Room removed from your cart successfully."));
    }
}