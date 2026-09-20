using FluentValidation;
using Triply.Application.DTOs.Cart;
using Triply.Application.Extensions;
using Triply.Application.Features.Cart.Commands.AddCartItem;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

/// <summary>Gets or sets the validated cart service.</summary>
/// <summary>Validates the requests before delegating to the cart service.</summary>
public class ValidatedCartService(
    ICartService inner,
    IEnumerable<IValidator<AddCartItemRequest>> addItemValidators) : ICartService
{
    /// <summary>Get user's cart.</summary>
    public async Task<Result<CartResponse>> GetAsync(Guid userId, CancellationToken cancellationToken = default)
        => await inner.GetAsync(userId, cancellationToken);
    /// <summary>Add item to the cart.</summary>
    public async Task<Result<CartResponse>> AddItemAsync(AddCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        await addItemValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.AddItemAsync(request, cancellationToken);
    }
    /// <summary>Remove item from the cart.</summary>
    public async Task<Result<bool>> RemoveItemAsync(Guid cartItemId, Guid userId,
        CancellationToken cancellationToken = default)
        => await inner.RemoveItemAsync(cartItemId, userId, cancellationToken);
    /// <summary>Clear the cart.</summary>
    public async Task<Result<bool>> ClearAsync(Guid userId, CancellationToken cancellationToken = default)
        => await inner.ClearAsync(userId, cancellationToken);
}