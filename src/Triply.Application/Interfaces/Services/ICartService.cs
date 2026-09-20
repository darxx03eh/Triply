using Triply.Application.DTOs.Cart;
using Triply.Application.Features.Cart.Commands.AddCartItem;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the cart operations.</summary>
public interface ICartService
{
    /// <summary>Gets the cart for specific user by his identifier operation.</summary>
    Task<Result<CartResponse>> GetAsync(Guid userId, CancellationToken cancellationToken = default);
    /// <summary>Add item to the user's cart operation.</summary>
    Task<Result<CartResponse>> AddItemAsync(AddCartItemRequest request,
        CancellationToken cancellationToken = default);
    /// <summary>Remove item to the user's cart operation.</summary>
    Task<Result<bool>> RemoveItemAsync(Guid cartItemId, Guid userId, CancellationToken cancellationToken = default);
    /// <summary>Clear items from the user's cart operation.</summary>
    Task<Result<bool>> ClearAsync(Guid userId, CancellationToken cancellationToken = default);
}