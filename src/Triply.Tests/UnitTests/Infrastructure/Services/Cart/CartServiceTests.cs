using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Triply.Application.Features.Cart.Commands.AddCartItem;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Domain.Results.Enums;
using Triply.Infrastructure.Services.Cart;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Cart;

public class CartServiceTests
{
    private readonly Mock<ICartRepository> _cart = new();
    private readonly Mock<IRoomRepository> _rooms = new();
    private readonly Mock<IDealRepository> _deals = new();
    private readonly Mock<IBookingRepository> _bookings = new();
    private readonly CartService _service;

    public CartServiceTests()
    {
        _service = new CartService(_cart.Object, _rooms.Object, _deals.Object, _bookings.Object,
            NullLogger<CartService>.Instance);
        _cart.Setup(x => x.GetUserCartAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _deals.Setup(x => x.GetActiveDiscountsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
    }

    [Fact]
    public async Task AddItemAsync_WhenRoomDoesNotExist_ReturnsNotFound()
    {
        _rooms.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Room)null!);

        var result = await _service.AddItemAsync(new AddCartItemRequest { UserId = Guid.NewGuid(), RoomId = Guid.NewGuid() });

        result.AssertFailure("ROOM_NOT_FOUND", ResultErrorType.NotFound);
        _cart.Verify(x => x.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenRoomIsBooked_ReturnsConflict()
    {
        var room = TestData.Room(TestData.Hotel(TestData.City()));
        _rooms.Setup(x => x.GetByIdAsync(room.RoomId, It.IsAny<CancellationToken>())).ReturnsAsync(room);
        _bookings.Setup(x => x.IsRoomBookedAsync(room.RoomId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.AddItemAsync(new AddCartItemRequest
        { UserId = Guid.NewGuid(), RoomId = room.RoomId, CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), Adults = 1 });

        result.AssertFailure("ROOM_ALREADY_BOOKED", ResultErrorType.Conflict);
    }

    [Fact]
    public async Task ClearAsync_AlwaysDelegatesToRepository()
    {
        var userId = Guid.NewGuid();
        var result = await _service.ClearAsync(userId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        _cart.Verify(x => x.ClearAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveItemAsync_WhenItemBelongsToAnotherUser_ReturnsNotFound()
    {
        var item = new CartItem { UserId = Guid.NewGuid() };
        _cart.Setup(x => x.GetByIdAsync(item.CartItemId, It.IsAny<CancellationToken>())).ReturnsAsync(item);

        var result = await _service.RemoveItemAsync(item.CartItemId, Guid.NewGuid());

        result.AssertFailure("CART_ITEM_NOT_FOUND", ResultErrorType.NotFound);
        _cart.Verify(x => x.DeleteAsync(It.IsAny<CartItem>()), Times.Never);
    }
}
