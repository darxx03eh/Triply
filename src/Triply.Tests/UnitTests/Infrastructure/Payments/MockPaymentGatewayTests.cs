using Microsoft.Extensions.Logging.Abstractions;
using Triply.Application.DTOs.Payments;
using Triply.Infrastructure.Payments;

namespace Triply.Tests.UnitTests.Infrastructure.Payments;

public class MockPaymentGatewayTests
{
    private readonly MockPaymentGateway _gateway = new(NullLogger<MockPaymentGateway>.Instance);

    [Fact]
    public async Task CreateCheckoutSessionAsync_ReturnsCompletedSessionWithoutRedirect()
    {
        var result = await _gateway.CreateCheckoutSessionAsync(new PaymentSessionRequest
        {
            ConfirmationNumber = "TRP-TEST", CustomerEmail = "guest@triply.com", Lines = [new PaymentLine("Room", 100m)]
        });

        Assert.True(result.IsCompleted);
        Assert.Null(result.CheckoutUrl);
        Assert.StartsWith("mock_", result.SessionId);
    }

    [Fact]
    public void ParseWebhookEvent_ValidPayload_ReturnsNormalizedEvent()
    {
        var result = _gateway.ParseWebhookEvent("{\"confirmationNumber\":\"TRP-TEST\",\"sessionId\":\"mock_1\",\"transactionId\":\"tx_1\",\"isPaid\":true}", null);

        Assert.NotNull(result);
        Assert.Equal("TRP-TEST", result.ConfirmationNumber);
        Assert.True(result.IsPaid);
    }

    [Fact]
    public void ParseWebhookEvent_InvalidPayload_ReturnsNull()
    {
        Assert.Null(_gateway.ParseWebhookEvent("not-json", null));
    }

    [Fact]
    public async Task RefundAsync_CompletesWithoutCallingAnExternalProvider()
    {
        await _gateway.RefundAsync("tx_1", 100m);
    }
}
