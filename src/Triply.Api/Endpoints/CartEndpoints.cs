using Triply.Api.Extensions;
using Triply.Api.Responses;
using Triply.Application.DTOs.Cart;
using Triply.Application.Features.Cart.Commands.AddCartItem;
using Triply.Application.Interfaces.Services;
using Triply.Infrastructure.Routes;

namespace Triply.Api.Endpoints;

/// <summary>Maps the cart endpoints.</summary>
public static class CartEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the cart endpoints.</summary>
        public void MapCartEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Cart")
                .RequireAuthorization();

            group.MapGet(Router.CartRoutes.Get, async (
                    ICurrentUserAccessor user, 
                    ICartService cartService, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await cartService.GetAsync(user.UserId, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetCart")
                .WithDisplayName("Get Cart")
                .WithSummary("Retrieves the signed-in user's cart")
                .WithDescription("""
                                 Retrieves the rooms in the signed-in user's cart with the nights, the running
                                 deal discount, the price of every room and the total price of the cart.
                                 Every item shows whether the room is still available for its dates.
                                 """)
                .Produces<ApiResponse<CartResponse>>(StatusCodes.Status200OK);

            group.MapPost(Router.CartRoutes.AddItem, async (
                    AddCartItemRequest request, 
                    ICurrentUserAccessor user, 
                    ICartService cartService,
                    CancellationToken cancellationToken) =>
                {
                    request.UserId = user.UserId;
                    var result = await cartService.AddItemAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("AddCartItem")
                .WithDisplayName("Add Cart Item")
                .WithSummary("Adds a room to the cart")
                .WithDescription("""
                                 Adds a room to the signed-in user's cart for the given dates and guests.
                                 Checks the room capacity and that the room is not booked for these dates.
                                 """)
                .Produces<ApiResponse<CartResponse>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .ProducesValidationProblem();

            group.MapDelete(Router.CartRoutes.RemoveItem, async (
                    Guid id, 
                    ICurrentUserAccessor user, 
                    ICartService cartService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await cartService.RemoveItemAsync(id, user.UserId, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("RemoveCartItem")
                .WithDisplayName("Remove Cart Item")
                .WithSummary("Removes a room from the cart")
                .WithDescription("""
                                 Removes a single item from the signed-in user's cart.
                                 """)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapDelete(Router.CartRoutes.Clear, async (
                    ICurrentUserAccessor user, 
                    ICartService cartService, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await cartService.ClearAsync(user.UserId, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("ClearCart")
                .WithDisplayName("Clear Cart")
                .WithSummary("Clears the cart")
                .WithDescription("""
                                 Removes all the items from the signed-in user's cart.
                                 """)
                .Produces(StatusCodes.Status204NoContent);
        }
    }
}