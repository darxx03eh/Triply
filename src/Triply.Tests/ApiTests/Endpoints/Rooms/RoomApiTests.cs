using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Cities;
using Triply.Application.DTOs.Hotels;
using Triply.Application.DTOs.Rooms;
using Triply.Domain.Enums.Rooms;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Rooms;

[Collection("Room API")]
[Trait("collection", "Room API")]
public sealed class RoomApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public RoomApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task GetRooms_ShouldReturnPagedRooms()
    {
        var response = await _client.GetAsync($"{Router.RoomRoutes.GetAll}?page=1&pageSize=10");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<RoomResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains(body!.Code, new[] { "ROOMS_FOUND", "ROOMS_EMPTY" });
        Assert.NotNull(body.Data);
        Assert.Equal(1, body.Data!.Page);
        Assert.Equal(10, body.Data.PageSize);
    }

    [Fact]
    public async Task GetRooms_WithInvalidPageSize_ShouldReturnValidationError()
    {
        var response = await _client.GetAsync($"{Router.RoomRoutes.GetAll}?page=0&pageSize=51");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.NotEmpty(body.Errors!.Fields);
    }

    [Fact]
    public async Task CreateRoom_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var request = ApiTestFixture.BuildRoomRequest(Guid.NewGuid());
        using var httpRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.RoomRoutes.Create, content: request);

        var response = await _client.SendAsync(httpRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateRoom_WithInvalidPayload_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        var request = new
        {
            HotelId = Guid.Empty,
            Number = "",
            RoomType = 99,
            AdultCapacity = 0,
            ChildCapacity = 11,
            PricePerNight = 0,
            Description = new string('x', 1001)
        };
        using var httpRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.RoomRoutes.Create, token, request);

        var response = await _client.SendAsync(httpRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.NotEmpty(body.Errors!.Fields);
    }

    [Fact]
    public async Task RoomCrud_WithValidData_ShouldGetUpdateAndDeleteRoom()
    {
        var token = await LoginAsAdminAsync();
        var hotelId = await CreateHotelAsync(token);
        var createRequest = ApiTestFixture.BuildRoomRequest(hotelId);

        using var createHttpRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.RoomRoutes.Create, token, createRequest);
        var createResponse = await _client.SendAsync(createHttpRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<RoomResponse>>();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(created?.Data);
        var room = created!.Data!;
        Assert.Equal(createRequest.Number, room.Number);
        Assert.Equal(hotelId, room.HotelId);
        Assert.NotEmpty(room.RowVersion);

        var roomEndpoint = Router.RoomRoutes.GetById.Replace("{id:guid}", room.RoomId.ToString());
        using var getRequest = ApiTestFixture.CreateRequest(HttpMethod.Get, roomEndpoint, token);
        var getResponse = await _client.SendAsync(getRequest);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ApiResponse<RoomResponse>>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(fetched?.Data);
        Assert.Equal(room.RoomId, fetched!.Data!.RoomId);

        var updateRequest = new
        {
            Number = $"{createRequest.Number}-U",
            RoomType = RoomType.Suite,
            AdultCapacity = (short)4,
            ChildCapacity = (short)2,
            PricePerNight = 210.00m,
            IsAvailable = false,
            Description = "Updated room description.",
            RowVersion = room.RowVersion
        };
        var updateEndpoint = Router.RoomRoutes.Update.Replace("{id:guid}", room.RoomId.ToString());
        using var updateHttpRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Put, updateEndpoint, token, updateRequest);
        var updateResponse = await _client.SendAsync(updateHttpRequest);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<RoomResponse>>();

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updated?.Data);
        Assert.Equal("ROOM_UPDATED", updated!.Code);
        Assert.Equal(updateRequest.Number, updated.Data!.Number);
        Assert.Equal(RoomType.Suite, updated.Data.RoomType);
        Assert.False(updated.Data.IsAvailable);

        var deleteEndpoint = Router.RoomRoutes.Delete.Replace("{id:guid}", room.RoomId.ToString());
        using var deleteRequest = ApiTestFixture.CreateRequest(HttpMethod.Delete, deleteEndpoint, token);
        var deleteResponse = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        using var deletedGetRequest = ApiTestFixture.CreateRequest(HttpMethod.Get, roomEndpoint, token);
        var deletedGetResponse = await _client.SendAsync(deletedGetRequest);
        var deletedBody = await deletedGetResponse.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, deletedGetResponse.StatusCode);
        Assert.NotNull(deletedBody);
        Assert.Equal("ROOM_NOT_FOUND", deletedBody!.Code);
    }

    [Fact]
    public async Task CreateRoom_WithDuplicateNumberInSameHotel_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        var hotelId = await CreateHotelAsync(token);
        var request = ApiTestFixture.BuildRoomRequest(hotelId);

        using var firstRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.RoomRoutes.Create, token, request);
        var firstResponse = await _client.SendAsync(firstRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        using var duplicateRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.RoomRoutes.Create, token, request);
        var response = await _client.SendAsync(duplicateRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("number", body.Errors!.Fields.Keys);
    }

    [Fact]
    public async Task GetRoomById_WithUnknownId_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.RoomRoutes.GetById.Replace("{id:guid}", Guid.NewGuid().ToString());
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Get, endpoint, token);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("ROOM_NOT_FOUND", body!.Code);
    }

    [Fact]
    public async Task GetHotelRooms_WithUnknownHotelId_ShouldReturnNotFound()
    {
        var endpoint = Router.HotelRoutes.GetRooms.Replace("{id:guid}", Guid.NewGuid().ToString());
        var response = await _client.GetAsync(endpoint);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("HOTEL_NOT_FOUND", body!.Code);
    }

    private async Task<Guid> CreateHotelAsync(string token)
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
        return hotel!.Data!.HotelId;
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
