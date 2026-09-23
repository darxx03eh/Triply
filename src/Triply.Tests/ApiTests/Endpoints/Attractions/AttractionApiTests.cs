using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.DTOs.Attractions;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Cities;
using Triply.Application.DTOs.Hotels;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Attractions;

[Collection("Attraction API")]
[Trait("collection", "Attraction API")]
public sealed class AttractionApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public AttractionApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task GetHotelAttractions_WithExistingHotel_ShouldReturnEmptyList()
    {
        var token = await LoginAsAdminAsync();
        var hotel = await CreateHotelAsync(token);
        var endpoint = Router.HotelRoutes.Attractions.Replace("{id:guid}", hotel.HotelId.ToString());

        var response = await _client.GetAsync(endpoint);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AttractionResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("ATTRACTIONS_FOUND", body!.Code);
        Assert.NotNull(body.Data);
        Assert.Empty(body.Data!);
    }

    [Fact]
    public async Task CreateAttraction_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post,
            Router.HotelRoutes.Attractions.Replace("{id:guid}", Guid.NewGuid().ToString()),
            content: new { Name = "Museum", Category = "Culture", DistanceKm = 1.25m });

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateAttraction_WithInvalidPayload_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        var hotel = await CreateHotelAsync(token);
        var endpoint = Router.HotelRoutes.Attractions.Replace("{id:guid}", hotel.HotelId.ToString());
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post,
            endpoint,
            token,
            new { Name = "", Category = new string('x', 51), DistanceKm = 100.001m });

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("name", body.Errors!.Fields.Keys);
        Assert.Contains("category", body.Errors.Fields.Keys);
        Assert.Contains("distanceKm", body.Errors.Fields.Keys);
    }

    [Fact]
    public async Task AttractionCrud_WithValidData_ShouldGetUpdateAndDeleteAttraction()
    {
        var token = await LoginAsAdminAsync();
        var hotel = await CreateHotelAsync(token);
        var createRequest = ApiTestFixture.BuildAttractionRequest(hotel.HotelId);
        var hotelAttractionsEndpoint = Router.HotelRoutes.Attractions
            .Replace("{id:guid}", hotel.HotelId.ToString());

        using var createRequestMessage = ApiTestFixture.CreateRequest(
            HttpMethod.Post, hotelAttractionsEndpoint, token, createRequest);
        var createResponse = await _client.SendAsync(createRequestMessage);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<AttractionResponse>>();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(created?.Data);
        Assert.Equal("ATTRACTION_CREATED", created!.Code);
        var attraction = created.Data!;
        Assert.Equal(hotel.HotelId, attraction.HotelId);
        Assert.Equal(createRequest.Name, attraction.Name);
        Assert.Equal(createRequest.Category, attraction.Category);
        Assert.Equal(createRequest.DistanceKm, attraction.DistanceKm);

        var attractionEndpoint = Router.AttractionRoutes.Update
            .Replace("{id:guid}", attraction.AttractionId.ToString());
        var updateRequest = new
        {
            Name = $"{createRequest.Name} Updated",
            Category = "Nature",
            DistanceKm = 4.75m
        };
        using var updateRequestMessage = ApiTestFixture.CreateRequest(
            HttpMethod.Put, attractionEndpoint, token, updateRequest);
        var updateResponse = await _client.SendAsync(updateRequestMessage);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<AttractionResponse>>();

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updated?.Data);
        Assert.Equal("ATTRACTION_UPDATED", updated!.Code);
        Assert.Equal(updateRequest.Name, updated.Data!.Name);
        Assert.Equal(updateRequest.Category, updated.Data.Category);
        Assert.Equal(updateRequest.DistanceKm, updated.Data.DistanceKm);

        var deleteEndpoint = Router.AttractionRoutes.Delete
            .Replace("{id:guid}", attraction.AttractionId.ToString());
        using var deleteRequest = ApiTestFixture.CreateRequest(HttpMethod.Delete, deleteEndpoint, token);
        var deleteResponse = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var listResponse = await _client.GetAsync(hotelAttractionsEndpoint);
        var listBody = await listResponse.Content
            .ReadFromJsonAsync<ApiResponse<IReadOnlyList<AttractionResponse>>>();

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.NotNull(listBody?.Data);
        Assert.DoesNotContain(listBody!.Data!, item => item.AttractionId == attraction.AttractionId);
    }

    [Fact]
    public async Task GetHotelAttractions_WithUnknownHotel_ShouldReturnNotFound()
    {
        var endpoint = Router.HotelRoutes.Attractions.Replace("{id:guid}", Guid.NewGuid().ToString());
        var response = await _client.GetAsync(endpoint);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("HOTEL_NOT_FOUND", body!.Code);
    }

    [Fact]
    public async Task UpdateAttraction_WithUnknownId_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.AttractionRoutes.Update.Replace("{id:guid}", Guid.NewGuid().ToString());
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Put,
            endpoint,
            token,
            new { Name = "Museum", Category = "Culture", DistanceKm = 1.25m });

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("ATTRACTION_NOT_FOUND", body!.Code);
    }

    [Fact]
    public async Task DeleteAttraction_WithUnknownId_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.AttractionRoutes.Delete.Replace("{id:guid}", Guid.NewGuid().ToString());
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Delete, endpoint, token);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("ATTRACTION_NOT_FOUND", body!.Code);
    }

    private async Task<HotelResponse> CreateHotelAsync(string token)
    {
        var cityRequest = ApiTestFixture.BuildCityRequest();
        using var cityHttpRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.CityRoutes.Create, token, cityRequest);
        var cityResponse = await _client.SendAsync(cityHttpRequest);
        var city = await cityResponse.Content.ReadFromJsonAsync<ApiResponse<CityResponse>>();

        Assert.Equal(HttpStatusCode.Created, cityResponse.StatusCode);
        Assert.NotNull(city?.Data);

        var hotelRequest = ApiTestFixture.BuildHotelRequest(city!.Data!.CityId);
        using var hotelHttpRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.HotelRoutes.Create, token, hotelRequest);
        var hotelResponse = await _client.SendAsync(hotelHttpRequest);
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<ApiResponse<HotelResponse>>();

        Assert.Equal(HttpStatusCode.Created, hotelResponse.StatusCode);
        Assert.NotNull(hotel?.Data);
        return hotel!.Data!;
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
