using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Cities;
using Triply.Application.DTOs.Hotels;
using Triply.Application.DTOs.Reviews;
using Triply.Infrastructure.Routes;
using Triply.Tests.ApiTests;
using Triply.Tests.IntegrationTests.Infrastructure;

namespace Triply.Tests.IntegrationTests.Endpoints.Reviews;

[Collection("Review Integration")]
[Trait("collection", "Review Integration")]
[Trait("Category", "IntegrationTests")]
public sealed class ReviewApiTests : IClassFixture<TriplyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ReviewApiTests(TriplyWebApplicationFactory factory)
        => _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

    [Fact]
    public async Task GetHotelReviews_WithExistingHotel_ShouldReturnEmptyPagedResult()
    {
        var token = await LoginAsAdminAsync();
        var hotel = await CreateHotelAsync(token);
        var endpoint = Router.HotelRoutes.Reviews.Replace("{id:guid}", hotel.HotelId.ToString());

        var response = await _client.GetAsync($"{endpoint}?page=1&pageSize=10");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ReviewResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("REVIEWS_EMPTY", body!.Code);
        Assert.NotNull(body.Data);
        Assert.Empty(body.Data!.Items);
        Assert.Equal(1, body.Data.Page);
        Assert.Equal(10, body.Data.PageSize);
        Assert.Equal(0, body.Data.TotalCount);
    }

    [Fact]
    public async Task GetHotelReviews_WithInvalidPagination_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        var hotel = await CreateHotelAsync(token);
        var endpoint = Router.HotelRoutes.Reviews.Replace("{id:guid}", hotel.HotelId.ToString());

        var response = await _client.GetAsync($"{endpoint}?page=0&pageSize=51");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("page", body.Errors!.Fields.Keys);
        Assert.Contains("pageSize", body.Errors.Fields.Keys);
    }

    [Fact]
    public async Task GetHotelReviews_WithUnknownHotel_ShouldReturnNotFound()
    {
        var endpoint = Router.HotelRoutes.Reviews.Replace("{id:guid}", Guid.NewGuid().ToString());

        var response = await _client.GetAsync(endpoint);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("HOTEL_NOT_FOUND", body!.Code);
    }

    [Fact]
    public async Task CreateReview_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var endpoint = Router.HotelRoutes.Reviews.Replace("{id:guid}", Guid.NewGuid().ToString());
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post,
            endpoint,
            content: new { Rating = 5, Title = "Excellent", Comment = "A wonderful stay overall." });

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateReview_WithInvalidPayload_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        var hotel = await CreateHotelAsync(token);
        var endpoint = Router.HotelRoutes.Reviews.Replace("{id:guid}", hotel.HotelId.ToString());
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post,
            endpoint,
            token,
            new { Rating = 0, Title = new string('x', 101), Comment = "short" });

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("rating", body.Errors!.Fields.Keys);
        Assert.Contains("title", body.Errors.Fields.Keys);
        Assert.Contains("comment", body.Errors.Fields.Keys);
    }

    [Fact]
    public async Task CreateReview_WithoutCompletedBooking_ShouldReturnForbidden()
    {
        var token = await LoginAsAdminAsync();
        var hotel = await CreateHotelAsync(token);
        var endpoint = Router.HotelRoutes.Reviews.Replace("{id:guid}", hotel.HotelId.ToString());
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Post,
            endpoint,
            token,
            new { Rating = 5, Title = "Excellent", Comment = "A wonderful stay overall." });

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("REVIEW_REQUIRES_COMPLETED_BOOKING", body!.Code);
    }

    [Fact]
    public async Task UpdateReview_WithUnknownId_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.ReviewRoutes.Update.Replace("{id:guid}", Guid.NewGuid().ToString());
        using var request = ApiTestFixture.CreateRequest(
            HttpMethod.Put,
            endpoint,
            token,
            new { Rating = 4, Title = "Updated", Comment = "The stay was very comfortable." });

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("REVIEW_NOT_FOUND", body!.Code);
    }

    [Fact]
    public async Task DeleteReview_WithUnknownId_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.ReviewRoutes.Delete.Replace("{id:guid}", Guid.NewGuid().ToString());
        using var request = ApiTestFixture.CreateRequest(HttpMethod.Delete, endpoint, token);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("REVIEW_NOT_FOUND", body!.Code);
    }

    private async Task<HotelResponse> CreateHotelAsync(string token)
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
