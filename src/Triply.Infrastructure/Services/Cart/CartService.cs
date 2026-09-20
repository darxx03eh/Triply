using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Cart;

/// <summary>Gets or sets the cart service.</summary>
/// <summary>Implements the cart operations.</summary>
public partial class CartService(
    ICartRepository cartRepository,
    IRoomRepository roomRepository,
    IDealRepository dealRepository,
    ILogger<CartService> logger) : ICartService{}