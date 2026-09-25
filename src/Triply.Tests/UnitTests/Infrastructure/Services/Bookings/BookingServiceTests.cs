using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Triply.Application.Features.Bookings.Commands.Checkout;
using Triply.Application.Interfaces.Payments;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results.Enums;
using Triply.Infrastructure.Services.Bookings;
using Triply.Tests.UnitTests.Common.Assertions;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Bookings;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookings = new();
    private readonly Mock<ICartRepository> _cart = new();
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        var deals = new Mock<IDealRepository>();
        var gateway = new Mock<IPaymentGateway>();
        _service = new BookingService(_bookings.Object, _cart.Object, deals.Object, gateway.Object,
            NullLogger<BookingService>.Instance);
    }

    [Fact]
    public async Task CheckoutAsync_WhenCartIsEmpty_ReturnsBusinessFailure()
    {
        var userId = Guid.NewGuid();
        _cart.Setup(x => x.GetUserCartAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _service.CheckoutAsync(new CheckoutRequest { UserId = userId });

        result.AssertFailure("CART_EMPTY", ResultErrorType.BusinessRule);
        _bookings.Verify(x => x.BeginSerializableTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByConfirmationNumberAsync_WhenNotFound_ReturnsNotFound()
    {
        _bookings.Setup(x => x.GetByConfirmationNumberAsync("TRP-MISSING", It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _service.GetByConfirmationNumberAsync("TRP-MISSING", Guid.NewGuid(), false);

        result.AssertFailure("BOOKING_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task CancelAsync_WhenNotFound_ReturnsNotFound()
    {
        _bookings.Setup(x => x.GetByConfirmationNumberAsync("TRP-MISSING", It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _service.CancelAsync("TRP-MISSING", Guid.NewGuid(), false);

        result.AssertFailure("BOOKING_NOT_FOUND", ResultErrorType.NotFound);
    }
}
