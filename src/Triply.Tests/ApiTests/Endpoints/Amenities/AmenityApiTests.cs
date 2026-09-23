using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Amenities;
using Triply.Application.DTOs.Cities;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Amenities;

[Collection("Amenity API")]
[Trait("collection", "Amenity API")]
public sealed class AmenityApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public AmenityApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task GetAmenities_ShouldReturnAllAmenities()
    {
        var response = await _client.GetAsync(Router.AmenityRoutes.GetAll);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AmenityResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("AMENITIES_FOUND", body!.Code);
        Assert.NotNull(body.Data);
    }

    [Fact]
    public async Task CreateAmenity_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.AmenityRoutes.Create, content: ApiTestFixture.BuildAmenityRequest());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateAmenity_WithInvalidPayload_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post,
            Router.AmenityRoutes.Create,
            token,
            new { Name = new string('x', 101) });

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("name", body.Errors!.Fields.Keys);
    }

    [Fact]
    public async Task AmenityCrud_WithValidData_ShouldGetUpdateAndDeleteAmenity()
    {
        var token = await LoginAsAdminAsync();
        var createRequest = ApiTestFixture.BuildAmenityRequest();

        using var createHttpRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.AmenityRoutes.Create, token, createRequest);
        var createResponse = await _client.SendAsync(createHttpRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<AmenityResponse>>();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(created?.Data);
        var amenity = created!.Data!;
        Assert.Equal(createRequest.Name, amenity.Name);

        var amenityEndpoint = Router.AmenityRoutes.GetById.Replace("{id:guid}", amenity.AmenityId.ToString());
        using var getRequest = ApiTestFixture.CreateRequest(HttpMethod.Get, amenityEndpoint, token);
        var getResponse = await _client.SendAsync(getRequest);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ApiResponse<AmenityResponse>>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(fetched?.Data);
        Assert.Equal(amenity.AmenityId, fetched!.Data!.AmenityId);

        var updateEndpoint = Router.AmenityRoutes.Update.Replace("{id:guid}", amenity.AmenityId.ToString());
        var updateRequest = new { Name = $"{createRequest.Name} Updated" };
        using var updateHttpRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Put, updateEndpoint, token, updateRequest);
        var updateResponse = await _client.SendAsync(updateHttpRequest);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<AmenityResponse>>();

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updated?.Data);
        Assert.Equal("AMENITY_UPDATED", updated!.Code);
        Assert.Equal(updateRequest.Name, updated.Data!.Name);

        var deleteEndpoint = Router.AmenityRoutes.Delete.Replace("{id:guid}", amenity.AmenityId.ToString());
        using var deleteRequest = ApiTestFixture.CreateRequest(HttpMethod.Delete, deleteEndpoint, token);
        var deleteResponse = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        using var deletedGetRequest = ApiTestFixture.CreateRequest(HttpMethod.Get, amenityEndpoint, token);
        var deletedGetResponse = await _client.SendAsync(deletedGetRequest);
        var deletedBody = await deletedGetResponse.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, deletedGetResponse.StatusCode);
        Assert.NotNull(deletedBody);
        Assert.Equal("AMENITY_NOT_FOUND", deletedBody!.Code);
    }

    [Fact]
    public async Task CreateAmenity_WithDuplicateName_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        var request = ApiTestFixture.BuildAmenityRequest();

        using var firstRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.AmenityRoutes.Create, token, request);
        var firstResponse = await _client.SendAsync(firstRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        using var duplicateRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.AmenityRoutes.Create, token, request);
        var response = await _client.SendAsync(duplicateRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("name", body.Errors!.Fields.Keys);
    }

    [Fact]
    public async Task GetAmenityById_WithUnknownId_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.AmenityRoutes.GetById.Replace("{id:guid}", Guid.NewGuid().ToString());
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Get, endpoint, token);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("AMENITY_NOT_FOUND", body!.Code);
    }

    [Fact]
    public async Task HotelAmenities_WithAdmin_ShouldSetAndGetAmenities()
    {
        var token = await LoginAsAdminAsync();
        var hotelId = await CreateHotelAsync(token);
        var firstAmenity = await CreateAmenityAsync(token);
        var secondAmenity = await CreateAmenityAsync(token);

        var endpoint = Router.HotelRoutes.Amenities.Replace("{id:guid}", hotelId.ToString());
        var setRequest = new SetHotelAmenitiesRequest
        {
            AmenityIds = [firstAmenity.AmenityId, secondAmenity.AmenityId]
        };
        using var setHttpRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Put, endpoint, token, setRequest);
        var setResponse = await _client.SendAsync(setHttpRequest);
        var setBody = await setResponse.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AmenityResponse>>>();

        Assert.Equal(HttpStatusCode.OK, setResponse.StatusCode);
        Assert.NotNull(setBody);
        Assert.Equal("HOTEL_AMENITIES_UPDATED", setBody!.Code);
        Assert.NotNull(setBody.Data);
        Assert.Equal(2, setBody.Data!.Count);

        var getResponse = await _client.GetAsync(endpoint);
        var getBody = await getResponse.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AmenityResponse>>>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getBody?.Data);
        Assert.Equal(
            setBody.Data.Select(amenity => amenity.AmenityId).OrderBy(id => id),
            getBody!.Data!.Select(amenity => amenity.AmenityId).OrderBy(id => id));
    }

    [Fact]
    public async Task SetHotelAmenities_WithDuplicateIds_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        var hotelId = await CreateHotelAsync(token);
        var amenity = await CreateAmenityAsync(token);
        var endpoint = Router.HotelRoutes.Amenities.Replace("{id:guid}", hotelId.ToString());

        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Put,
            endpoint,
            token,
            new SetHotelAmenitiesRequest { AmenityIds = [amenity.AmenityId, amenity.AmenityId] });
        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
    }

    private async Task<AmenityResponse> CreateAmenityAsync(string token)
    {
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.AmenityRoutes.Create, token, ApiTestFixture.BuildAmenityRequest());
        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AmenityResponse>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body?.Data);
        return body!.Data!;
    }

    private async Task<Guid> CreateHotelAsync(string token)
    {
        var cityRequest = ApiTestFixture.BuildCityRequest();
        using var cityRequestMessage = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.CityRoutes.Create, token, cityRequest);
        var cityResponse = await _client.SendAsync(cityRequestMessage);
        var cityBody = await cityResponse.Content.ReadFromJsonAsync<ApiResponse<CityResponse>>();

        Assert.Equal(HttpStatusCode.Created, cityResponse.StatusCode);
        Assert.NotNull(cityBody?.Data);

        var hotelRequest = ApiTestFixture.BuildHotelRequest(cityBody!.Data!.CityId);
        using var hotelRequestMessage = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.HotelRoutes.Create, token, hotelRequest);
        var hotelResponse = await _client.SendAsync(hotelRequestMessage);
        var hotelBody = await hotelResponse.Content.ReadFromJsonAsync<ApiResponse<HotelResponse>>();

        Assert.Equal(HttpStatusCode.Created, hotelResponse.StatusCode);
        Assert.NotNull(hotelBody?.Data);
        return hotelBody!.Data!.HotelId;
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
