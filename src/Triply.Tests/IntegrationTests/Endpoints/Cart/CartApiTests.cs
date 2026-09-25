using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Triply.Api.Responses;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Cart;
using Triply.Infrastructure.Routes;
using Triply.Tests.ApiTests;
using Triply.Tests.IntegrationTests.Infrastructure;

namespace Triply.Tests.IntegrationTests.Endpoints.Cart;

[Collection("Cart Integration")]
[Trait("collection", "Cart Integration")]
[Trait("Category", "IntegrationTests")]
public sealed class CartApiTests : IClassFixture<TriplyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CartApiTests(TriplyWebApplicationFactory factory)
        => _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

    [Fact]
    public async Task GetCart_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync(Router.CartRoutes.Get);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCart_WithAuthentication_ShouldReturnCart()
    {
        var token = await LoginAsAdminAsync();
        using var clearRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Delete, Router.CartRoutes.Clear, token);
        await _client.SendAsync(clearRequest);

        using var request = ApiTestFixture.CreateRequest(HttpMethod.Get, Router.CartRoutes.Get, token);
        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CartResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("CART_EMPTY", body!.Code);
        Assert.NotNull(body.Data);
        Assert.Empty(body.Data!.Items);
        Assert.Equal(0, body.Data.ItemsCount);
        Assert.Equal(0m, body.Data.TotalPrice);
    }

    [Fact]
    public async Task AddCartItem_WithInvalidPayload_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post,
            Router.CartRoutes.AddItem,
            token,
            new
            {
                RoomId = Guid.Empty,
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
                Adults = 0,
                Children = 21
            });

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("roomId", body.Errors!.Fields.Keys);
        Assert.Contains("checkIn", body.Errors.Fields.Keys);
        Assert.Contains("checkOut", body.Errors.Fields.Keys);
        Assert.Contains("adults", body.Errors.Fields.Keys);
        Assert.Contains("children", body.Errors.Fields.Keys);
    }

    [Fact]
    public async Task AddCartItem_WithUnknownRoom_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post,
            Router.CartRoutes.AddItem,
            token,
            new
            {
                RoomId = Guid.NewGuid(),
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                Adults = 2,
                Children = 0
            });

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("roomId", body.Errors!.Fields.Keys);
    }

    [Fact]
    public async Task RemoveCartItem_WithUnknownId_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.CartRoutes.RemoveItem.Replace("{id:guid}", Guid.NewGuid().ToString());
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Delete, endpoint, token);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("CART_ITEM_NOT_FOUND", body!.Code);
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
