using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Bookings;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Bookings;

[Collection("Booking API")]
[Trait("collection", "Booking API")]
[Trait("Category", "ApiTests")]
public sealed class BookingApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public BookingApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task GetMyBookings_WithAuthentication_ShouldReturnPagedBookings()
    {
        var token = await LoginAsAdminAsync();
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Get, $"{Router.UserRoutes.Bookings}?page=1&pageSize=10", token);
        var response = await _client.SendAsync(request);
        var body = await response.Content
            .ReadFromJsonAsync<ApiResponse<PagedResult<BookingResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains(body!.Code, new[] { "BOOKINGS_FOUND", "BOOKINGS_EMPTY" });
        Assert.NotNull(body.Data);
        Assert.Equal(1, body.Data!.Page);
        Assert.Equal(10, body.Data.PageSize);
    }

    [Fact]
    public async Task GetMyBookings_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync(Router.UserRoutes.Bookings);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyBookings_WithInvalidPagination_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Get, $"{Router.UserRoutes.Bookings}?page=0&pageSize=51", token);
        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("page", body.Errors!.Fields.Keys);
        Assert.Contains("pageSize", body.Errors.Fields.Keys);
    }

    [Fact]
    public async Task Checkout_WithInvalidPayload_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post,
            Router.BookingRoutes.Checkout,
            token,
            new
            {
                GuestFullName = "",
                GuestEmail = "invalid-email",
                GuestPhoneNumber = "123",
                SpecialRequests = new string('x', 2001)
            });

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("guestFullName", body.Errors!.Fields.Keys);
        Assert.Contains("guestEmail", body.Errors.Fields.Keys);
        Assert.Contains("guestPhoneNumber", body.Errors.Fields.Keys);
        Assert.Contains("specialRequests", body.Errors.Fields.Keys);
    }

    [Fact]
    public async Task GetBookingByConfirmationNumber_WithUnknownNumber_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.BookingRoutes.GetByConfirmationNumber
            .Replace("{confirmationNumber}", "TRP-UNKNOWN-0001");
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Get, endpoint, token);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("BOOKING_NOT_FOUND", body!.Code);
    }

    [Fact]
    public async Task CancelBooking_WithUnknownNumber_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.BookingRoutes.Cancel
            .Replace("{confirmationNumber}", "TRP-UNKNOWN-0002");
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Post, endpoint, token);

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
