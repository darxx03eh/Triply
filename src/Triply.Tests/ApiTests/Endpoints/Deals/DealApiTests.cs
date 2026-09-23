using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Cities;
using Triply.Application.DTOs.Deals;
using Triply.Application.DTOs.Hotels;
using Triply.Application.DTOs.Rooms;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Deals;

[Collection("Deal API")]
[Trait("collection", "Deal API")]
public sealed class DealApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public DealApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task GetDeals_WithAuthentication_ShouldReturnPagedDeals()
    {
        var token = await LoginAsAdminAsync();

        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Get, $"{Router.DealRoutes.GetAll}?page=1&pageSize=10", token);
        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<DealResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains(body!.Code, new[] { "DEALS_FOUND", "DEALS_EMPTY" });
        Assert.NotNull(body.Data);
        Assert.Equal(1, body.Data!.Page);
        Assert.Equal(10, body.Data.PageSize);
    }

    [Fact]
    public async Task DealEndpoints_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync(Router.DealRoutes.GetAll);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetDeals_WithInvalidPagination_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Get, $"{Router.DealRoutes.GetAll}?page=0&pageSize=51", token);
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
    public async Task CreateDeal_WithInvalidPayload_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post,
            Router.DealRoutes.Create,
            token,
            new
            {
                RoomId = Guid.Empty,
                Title = "",
                DiscountPercentage = 0m,
                StartsAt = DateTime.UtcNow.AddDays(2),
                EndsAt = DateTime.UtcNow.AddDays(1)
            });

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("roomId", body.Errors!.Fields.Keys);
        Assert.Contains("title", body.Errors.Fields.Keys);
        Assert.Contains("discountPercentage", body.Errors.Fields.Keys);
        Assert.Contains("endsAt", body.Errors.Fields.Keys);
    }

    [Fact]
    public async Task DealCrud_WithValidData_ShouldGetUpdateAndDeleteDeal()
    {
        var token = await LoginAsAdminAsync();
        var room = await CreateRoomAsync(token);
        var createRequest = ApiTestFixture.BuildDealRequest(room.RoomId);

        using var createRequestMessage = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.DealRoutes.Create, token, createRequest);
        var createResponse = await _client.SendAsync(createRequestMessage);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<DealResponse>>();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(created?.Data);
        Assert.Equal("DEAL_CREATED", created!.Code);
        var deal = created.Data!;
        Assert.Equal(createRequest.RoomId, deal.RoomId);
        Assert.Equal(createRequest.Title, deal.Title);
        Assert.Equal(createRequest.DiscountPercentage, deal.DiscountPercentage);
        Assert.Equal(createRequest.IsFeatured, deal.IsFeatured);

        var dealEndpoint = Router.DealRoutes.GetById.Replace("{id:guid}", deal.DealId.ToString());
        using var getRequest = ApiTestFixture.CreateRequest(HttpMethod.Get, dealEndpoint, token);
        var getResponse = await _client.SendAsync(getRequest);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ApiResponse<DealResponse>>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(fetched?.Data);
        Assert.Equal("DEAL_FOUND", fetched!.Code);
        Assert.Equal(deal.DealId, fetched.Data!.DealId);

        var updateRequest = new
        {
            Title = $"{createRequest.Title} Updated",
            DiscountPercentage = 25.00m,
            StartsAt = createRequest.StartsAt.AddDays(1),
            EndsAt = createRequest.EndsAt.AddDays(1),
            IsFeatured = false
        };
        var updateEndpoint = Router.DealRoutes.Update.Replace("{id:guid}", deal.DealId.ToString());
        using var updateRequestMessage = ApiTestFixture.CreateRequest(
            HttpMethod.Put, updateEndpoint, token, updateRequest);
        var updateResponse = await _client.SendAsync(updateRequestMessage);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<DealResponse>>();

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updated?.Data);
        Assert.Equal("DEAL_UPDATED", updated!.Code);
        Assert.Equal(updateRequest.Title, updated.Data!.Title);
        Assert.Equal(updateRequest.DiscountPercentage, updated.Data.DiscountPercentage);
        Assert.Equal(updateRequest.IsFeatured, updated.Data.IsFeatured);

        var deleteEndpoint = Router.DealRoutes.Delete.Replace("{id:guid}", deal.DealId.ToString());
        using var deleteRequest = ApiTestFixture.CreateRequest(HttpMethod.Delete, deleteEndpoint, token);
        var deleteResponse = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        using var deletedGetRequest = ApiTestFixture.CreateRequest(HttpMethod.Get, dealEndpoint, token);
        var deletedGetResponse = await _client.SendAsync(deletedGetRequest);
        var deletedBody = await deletedGetResponse.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, deletedGetResponse.StatusCode);
        Assert.NotNull(deletedBody);
        Assert.Equal("DEAL_NOT_FOUND", deletedBody!.Code);
    }

    [Fact]
    public async Task CreateDeal_WithUnknownRoom_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var requestModel = ApiTestFixture.BuildDealRequest(Guid.NewGuid());
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.DealRoutes.Create, token, requestModel);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("ROOM_NOT_FOUND", body!.Code);
    }

    [Fact]
    public async Task CreateDeal_WithOverlappingPeriod_ShouldReturnConflict()
    {
        var token = await LoginAsAdminAsync();
        var room = await CreateRoomAsync(token);
        var firstRequestModel = ApiTestFixture.BuildDealRequest(room.RoomId);

        using var firstRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.DealRoutes.Create, token, firstRequestModel);
        var firstResponse = await _client.SendAsync(firstRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var overlappingRequestModel = ApiTestFixture.BuildDealRequest(
            room.RoomId, firstRequestModel.StartsAt.AddDays(2));
        using var overlappingRequest = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.DealRoutes.Create, token, overlappingRequestModel);
        var response = await _client.SendAsync(overlappingRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("DEAL_OVERLAPPING", body!.Code);
    }

    [Fact]
    public async Task GetDealById_WithUnknownId_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.DealRoutes.GetById.Replace("{id:guid}", Guid.NewGuid().ToString());
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Get, endpoint, token);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("DEAL_NOT_FOUND", body!.Code);
    }

    private async Task<RoomResponse> CreateRoomAsync(string token)
    {
        var cityRequest = ApiTestFixture.BuildCityRequest();
        using var cityRequestMessage = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.CityRoutes.Create, token, cityRequest);
        var cityResponse = await _client.SendAsync(cityRequestMessage);
        var city = await cityResponse.Content.ReadFromJsonAsync<ApiResponse<CityResponse>>();

        Assert.Equal(HttpStatusCode.Created, cityResponse.StatusCode);
        Assert.NotNull(city?.Data);

        var hotelRequest = ApiTestFixture.BuildHotelRequest(city!.Data!.CityId);
        using var hotelRequestMessage = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.HotelRoutes.Create, token, hotelRequest);
        var hotelResponse = await _client.SendAsync(hotelRequestMessage);
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<ApiResponse<HotelResponse>>();

        Assert.Equal(HttpStatusCode.Created, hotelResponse.StatusCode);
        Assert.NotNull(hotel?.Data);

        var roomRequest = ApiTestFixture.BuildRoomRequest(hotel!.Data!.HotelId);
        using var roomRequestMessage = ApiTestFixture.CreateRequest(
            HttpMethod.Post, Router.RoomRoutes.Create, token, roomRequest);
        var roomResponse = await _client.SendAsync(roomRequestMessage);
        var room = await roomResponse.Content.ReadFromJsonAsync<ApiResponse<RoomResponse>>();

        Assert.Equal(HttpStatusCode.Created, roomResponse.StatusCode);
        Assert.NotNull(room?.Data);
        return room!.Data!;
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
