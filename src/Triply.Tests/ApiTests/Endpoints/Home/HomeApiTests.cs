using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Deals;
using Triply.Application.DTOs.Home;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Home;

[Collection("Home API")]
[Trait("collection", "Home API")]
public sealed class HomeApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public HomeApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task GetFeaturedDeals_ShouldReturnFeaturedDealsOrEmptyResult()
    {
        var response = await _client.GetAsync($"{Router.DealRoutes.Featured}?count=5");
        var body = await response.Content
            .ReadFromJsonAsync<ApiResponse<IReadOnlyList<FeaturedDealResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains(body!.Code, new[] { "FEATURED_DEALS_FOUND", "FEATURED_DEALS_EMPTY" });
        Assert.NotNull(body.Data);
        Assert.True(body.Data!.Count <= 5);
    }

    [Fact]
    public async Task GetTrendingCities_ShouldReturnTrendingCities()
    {
        var response = await _client.GetAsync($"{Router.CityRoutes.Trending}?count=5");
        var body = await response.Content
            .ReadFromJsonAsync<ApiResponse<IReadOnlyList<TrendingCityResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("TRENDING_CITIES_FOUND", body!.Code);
        Assert.NotNull(body.Data);
        Assert.True(body.Data!.Count <= 5);
    }

    [Fact]
    public async Task GetRecentHotels_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync(Router.UserRoutes.RecentHotels);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetRecentHotels_WithAuthentication_ShouldReturnRecentHotelsOrEmptyResult()
    {
        var token = await LoginAsAdminAsync();
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Get, $"{Router.UserRoutes.RecentHotels}?count=5", token);
        var response = await _client.SendAsync(request);
        var body = await response.Content
            .ReadFromJsonAsync<ApiResponse<IReadOnlyList<RecentHotelResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains(body!.Code, new[] { "RECENT_HOTELS_FOUND", "RECENT_HOTELS_EMPTY" });
        Assert.NotNull(body.Data);
        Assert.True(body.Data!.Count <= 5);
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
