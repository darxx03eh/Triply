using Microsoft.Extensions.Logging;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cart;

public partial class CartService
{
    /// <summary>Clear user's cart.</summary>
    public async Task<Result<bool>> ClearAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var removed = await cartRepository.ClearAsync(userId, cancellationToken);
        logger.LogInformation("Cart of user {UserId} cleared ({Count} items removed)", userId, removed);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ("CART_CLEARED", "Your cart was cleared successfully."));
    }
}