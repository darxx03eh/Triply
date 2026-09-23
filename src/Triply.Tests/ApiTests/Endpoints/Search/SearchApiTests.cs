using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Cities;
using Triply.Application.DTOs.Hotels;
using Triply.Application.DTOs.Search;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Domain.Enums.Hotels;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Search;

[Collection("Search API")]
public sealed class SearchApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public SearchApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task SearchHotels_WithDefaults_ShouldReturnSearchMetadata()
    {
        var response = await _client.GetAsync(Router.SearchRoutes.Hotels);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SearchHotelsResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains(body!.Code, new[] { "SEARCH_RESULTS_FOUND", "SEARCH_NO_RESULTS" });
        Assert.NotNull(body.Data);
        Assert.Equal(1, body.Data!.Page);
        Assert.Equal(10, body.Data.PageSize);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), body.Data.CheckIn);
        Assert.Equal(body.Data.CheckIn.AddDays(1), body.Data.CheckOut);
        Assert.Equal(1, body.Data.Nights);
        Assert.Equal(2, body.Data.Adults);
        Assert.Equal(0, body.Data.Children);
        Assert.Equal(1, body.Data.Rooms);
    }

    [Fact]
    public async Task SearchHotels_WithInvalidParameters_ShouldReturnValidationError()
    {
        var endpoint = $"{Router.SearchRoutes.Hotels}?adults=0&rooms=2&minPrice=100&maxPrice=50" +
                       "&stars=6&types=Invalid&page=0&pageSize=51&sort=invalid";
        var response = await _client.GetAsync(endpoint);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.NotEmpty(body.Errors!.Fields);
    }

    [Fact]
    public async Task SearchHotels_WithPastCheckIn_ShouldReturnValidationError()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var endpoint = $"{Router.SearchRoutes.Hotels}?checkIn={yesterday:yyyy-MM-dd}";
        var response = await _client.GetAsync(endpoint);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("checkIn", body.Errors!.Fields.Keys);
    }

    [Fact]
    public async Task SearchHotels_WithAvailableHotelAndFilters_ShouldReturnMatchingHotel()
    {
        var token = await LoginAsAdminAsync();
        var city = await CreateCityAsync(token);
        var hotel = await CreateHotelAsync(token, city.CityId);
        await CreateRoomAsync(token, hotel.HotelId);

        var endpoint = $"{Router.SearchRoutes.Hotels}?q={Uri.EscapeDataString(hotel.Name)}" +
                       $"&cityId={city.CityId}&types={HotelType.Boutique}&stars=4" +
                       "&adults=2&children=1&rooms=1&sort=price_asc&page=1&pageSize=10";
        var response = await _client.GetAsync(endpoint);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SearchHotelsResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("SEARCH_RESULTS_FOUND", body!.Code);
        Assert.NotNull(body.Data);
        Assert.Contains(body.Data!.Items, item => item.HotelId == hotel.HotelId);

        var result = Assert.Single(body.Data.Items, item => item.HotelId == hotel.HotelId);
        Assert.Equal(hotel.Name, result.Name);
        Assert.Equal(city.CityId, result.CityId);
        Assert.Equal(HotelType.Boutique, result.HotelType);
        Assert.Equal(125.50m, result.MinPricePerNight);
        Assert.Equal(125.50m, result.TotalPrice);
        Assert.Equal(1, result.AvailableRooms);
    }

    private async Task<CityResponse> CreateCityAsync(string token)
    {
        var cityRequest = ApiTestFixture.BuildCityRequest();
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.CityRoutes.Create, token, cityRequest);
        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CityResponse>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body?.Data);
        return body!.Data!;
    }

    private async Task<HotelResponse> CreateHotelAsync(string token, Guid cityId)
    {
        var hotelRequest = ApiTestFixture.BuildHotelRequest(cityId);
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.HotelRoutes.Create, token, hotelRequest);
        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<HotelResponse>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body?.Data);
        return body!.Data!;
    }

    private async Task CreateRoomAsync(string token, Guid hotelId)
    {
        var roomRequest = ApiTestFixture.BuildRoomRequest(hotelId);
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.RoomRoutes.Create, token, roomRequest);
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
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
