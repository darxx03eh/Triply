using MessageQueue.IRabbitMQ;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Triply.Application.DTOs.Payments;
using Triply.Application.Interfaces.Payments;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Application.Options;
using Triply.Domain.Entities;
using Triply.Domain.Results.Enums;
using Triply.Infrastructure.Services.Payments;
using Triply.Tests.UnitTests.Common.Assertions;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Payments;

public class PaymentServiceTests
{
    private readonly Mock<IBookingRepository> _bookings = new();
    private readonly Mock<IPaymentGateway> _gateway = new();
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _gateway.SetupGet(x => x.Name).Returns("mock");
        _service = new PaymentService(_bookings.Object, new Mock<IGenericRepository<Payment>>().Object,
            _gateway.Object, new Mock<IMessagePublisher>().Object,
            Options.Create(new PaymentOptions { Currency = "usd" }), NullLogger<PaymentService>.Instance);
    }

    [Fact]
    public async Task PayAsync_WhenBookingDoesNotExist_ReturnsNotFound()
    {
        _bookings.Setup(x => x.GetByConfirmationNumberAsync("TRP-MISSING", It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _service.PayAsync("TRP-MISSING", Guid.NewGuid());

        result.AssertFailure("BOOKING_NOT_FOUND", ResultErrorType.NotFound);
        _gateway.Verify(x => x.CreateCheckoutSessionAsync(It.IsAny<PaymentSessionRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleWebhookAsync_WhenPayloadIsInvalid_ReturnsBusinessFailure()
    {
        _gateway.Setup(x => x.ParseWebhookEvent("invalid", It.IsAny<string?>())).Returns((PaymentWebhookEvent?)null);

        var result = await _service.HandleWebhookAsync("invalid", null);

        result.AssertFailure("PAYMENT_WEBHOOK_INVALID", ResultErrorType.BusinessRule);
        _bookings.Verify(x => x.GetByConfirmationNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleWebhookAsync_WhenBookingDoesNotExist_ReturnsNotFound()
    {
        _gateway.Setup(x => x.ParseWebhookEvent("event", null)).Returns(new PaymentWebhookEvent
        { ConfirmationNumber = "TRP-MISSING", SessionId = "session", IsPaid = true });
        _bookings.Setup(x => x.GetByConfirmationNumberAsync("TRP-MISSING", It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _service.HandleWebhookAsync("event", null);

        result.AssertFailure("BOOKING_NOT_FOUND", ResultErrorType.NotFound);
    }
}
