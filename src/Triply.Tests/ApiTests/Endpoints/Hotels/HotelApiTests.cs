using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Cities;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Domain.Enums.Hotels;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Hotels;

[Collection("Hotel API")]
[Trait("collection", "Hotel API")]
public sealed class HotelApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public HotelApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task GetHotels_ShouldReturnPagedHotels()
    {
        var response = await _client.GetAsync($"{Router.HotelRoutes.GetAll}?page=1&pageSize=10");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<HotelSummaryResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains(body!.Code, new[] { "HOTELS_FOUND", "HOTELS_EMPTY" });
        Assert.NotNull(body.Data);
        Assert.Equal(1, body.Data!.Page);
        Assert.Equal(10, body.Data.PageSize);
    }

    [Fact]
    public async Task GetHotels_WithInvalidPageSize_ShouldReturnValidationError()
    {
        var response = await _client.GetAsync($"{Router.HotelRoutes.GetAll}?page=0&pageSize=51");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.NotEmpty(body.Errors!.Fields);
    }

    [Fact]
    public async Task CreateHotel_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var request = new
        {
            Name = $"Unauthenticated Hotel {Guid.NewGuid():N}",
            CityId = Guid.NewGuid(),
            StarRating = (byte)4,
            HotelType = HotelType.Boutique,
            Address = "1 Test Street",
            Description = "Test hotel",
        };

        using var httpRequest = ApiTestFixture.CreateRequest(HttpMethod.Post, 
            Router.HotelRoutes.Create, content: request);
        var response = await _client.SendAsync(httpRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateHotel_WithInvalidPayload_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        var request = new
        {
            Name = "",
            CityId = Guid.Empty,
            StarRating = (byte)0,
            HotelType = (HotelType)99,
            Address = "",
            Description = new string('x', 2001),
            Latitude = 91m,
            Longitude = 181m
        };

        using var httpRequest = ApiTestFixture.CreateRequest(HttpMethod.Post, 
            Router.HotelRoutes.Create, token, request);
        var response = await _client.SendAsync(httpRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.NotEmpty(body.Errors!.Fields);
    }

    [Fact]
    public async Task HotelCrud_WithValidData_ShouldGetUpdateAndDeleteHotel()
    {
        var token = await LoginAsAdminAsync();
        var city = await CreateCityAsync(token);
        var createRequest = ApiTestFixture.BuildHotelRequest(city);

        using var createHttpRequest = ApiTestFixture.CreateRequest(HttpMethod.Post, 
            Router.HotelRoutes.Create, token, createRequest);
        var createResponse = await _client.SendAsync(createHttpRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<HotelResponse>>();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(created?.Data);
        var hotel = created!.Data!;
        Assert.Equal(createRequest.Name, hotel.Name);
        Assert.Equal(city, hotel.CityId);
        Assert.NotEmpty(hotel.RowVersion);

        var hotelEndpoint = Router.HotelRoutes.GetById.Replace("{id:guid}", hotel.HotelId.ToString());
        var updateEndpoint = Router.HotelRoutes.Update.Replace("{id:guid}", hotel.HotelId.ToString());
        var deleteEndpoint = Router.HotelRoutes.Delete.Replace("{id:guid}", hotel.HotelId.ToString());

        using var getRequest = ApiTestFixture.CreateRequest(HttpMethod.Get, hotelEndpoint);
        var getResponse = await _client.SendAsync(getRequest);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ApiResponse<HotelResponse>>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(fetched?.Data);
        Assert.Equal(hotel.HotelId, fetched!.Data!.HotelId);

        var updateRequest = new
        {
            Name = $"{createRequest.Name} Updated",
            CityId = city,
            StarRating = (byte)5,
            HotelType = HotelType.Luxury,
            Address = "2 Updated Street",
            Description = "Updated hotel description.",
            RowVersion = hotel.RowVersion
        };
        using var updateHttpRequest = ApiTestFixture.CreateRequest(HttpMethod.Put, 
            updateEndpoint, token, updateRequest);
        var updateResponse = await _client.SendAsync(updateHttpRequest);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<HotelResponse>>();

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updated?.Data);
        Assert.Equal("HOTEL_UPDATED", updated!.Code);
        Assert.Equal(updateRequest.Name, updated.Data!.Name);
        Assert.Equal(HotelType.Luxury, updated.Data.HotelType);

        using var deleteRequest = ApiTestFixture.CreateRequest(HttpMethod.Delete, deleteEndpoint, token);
        var deleteResponse = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        using var deletedGetRequest = ApiTestFixture.CreateRequest(HttpMethod.Get, hotelEndpoint);
        var deletedGetResponse = await _client.SendAsync(deletedGetRequest);
        var deletedBody = await deletedGetResponse.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, deletedGetResponse.StatusCode);
        Assert.NotNull(deletedBody);
        Assert.Equal("HOTEL_NOT_FOUND", deletedBody!.Code);
    }

    [Fact]
    public async Task CreateHotel_WithDuplicateNameInSameCity_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        var city = await CreateCityAsync(token);
        var request = new
        {
            Name = $"Duplicate Hotel {Guid.NewGuid():N}",
            CityId = city,
            StarRating = (byte)3,
            HotelType = HotelType.Budget,
            Address = "3 Test Street",
            Description = "Duplicate hotel test"
        };

        using var firstRequest = ApiTestFixture.CreateRequest(HttpMethod.Post, Router.HotelRoutes.Create, 
            token, request);
        var firstResponse = await _client.SendAsync(firstRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        using var duplicateRequest = ApiTestFixture.CreateRequest(HttpMethod.Post, 
            Router.HotelRoutes.Create, token, request);
        var response = await _client.SendAsync(duplicateRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("name", body.Errors!.Fields.Keys);
    }

    [Fact]
    public async Task GetHotelById_WithUnknownId_ShouldReturnNotFound()
    {
        var endpoint = Router.HotelRoutes.GetById.Replace("{id:guid}", Guid.NewGuid().ToString());
        var response = await _client.GetAsync(endpoint);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("HOTEL_NOT_FOUND", body!.Code);
    }

    private async Task<Guid> CreateCityAsync(string token)
    {
        var cityRequest = new CreateCityRequest
        {
            Name = $"Test City For Hotel {Guid.NewGuid():N}",
            Country = "Testland",
            PostOffice = "12345"
        };
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Post, 
            Router.CityRoutes.Create, token, cityRequest);
        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CityResponse>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body?.Data);
        return body!.Data!.CityId;
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
