using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Payments;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Payments;

[Collection("Payment API")]
[Trait("collection", "Payment API")]
public sealed class PaymentApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public PaymentApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task PayBooking_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var endpoint = Router.BookingRoutes.Pay.Replace("{confirmationNumber}", "TRP-UNKNOWN-0003");
        var response = await _client.PostAsync(endpoint, content: null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PayBooking_WithUnknownBooking_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.BookingRoutes.Pay.Replace("{confirmationNumber}", "TRP-UNKNOWN-0004");
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Post, endpoint, token);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("BOOKING_NOT_FOUND", body!.Code);
    }

    [Fact]
    public async Task PaymentWebhook_WithInvalidPayload_ShouldReturnBadRequest()
    {
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.PaymentRoutes.Webhook, content: "not-a-payment-event");
        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("PAYMENT_WEBHOOK_INVALID", body!.Code);
    }

    [Fact]
    public async Task GetInvoice_WithUnknownBooking_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.BookingRoutes.Invoice.Replace("{confirmationNumber}", "TRP-UNKNOWN-0005");
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Get, endpoint, token);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("BOOKING_NOT_FOUND", body!.Code);
    }

    private async Task<string> LoginAsAdminAsync()
    {
        var response = await _client.PostAsJsonAsync(Router.AuthenticationRoutes.Login, new
        {
            Identifier = "admin@triply.com",
            Password = "admin003+-"
        });
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LoginApiResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body?.Data);
        return body!.Data!.Access;
    }
}
