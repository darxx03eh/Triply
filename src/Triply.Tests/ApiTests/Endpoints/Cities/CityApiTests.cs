using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Authentications;
using Triply.Application.DTOs.Cities;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Cities;

[Collection("City API")]
[Trait("collection", "City API")]
public class CityApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public CityApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task GetCities_ShouldReturnPagedCities()
    {
        var response = await _client.GetAsync($"{Router.CityRoutes.GetAll}?page=1&pageSize=10");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<CityResponse>>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains(body!.Code, new[] { "CITIES_FOUND", "CITIES_EMPTY" });
        Assert.NotNull(body.Data);
        Assert.Equal(1, body.Data!.Page);
        Assert.Equal(10, body.Data.PageSize);
        Assert.NotNull(body.Data.Items);
    }

    [Fact]
    public async Task GetCities_WithInvalidPageSize_ShouldReturnValidationError()
    {
        var response = await _client.GetAsync($"{Router.CityRoutes.GetAll}?page=0&pageSize=51");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.NotEmpty(body.Errors!.Fields);
    }

    [Fact]
    public async Task CreateCity_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Router.CityRoutes.Create)
        {
            Content = JsonContent.Create(new
            {
                Name = $"Unauthenticated City {Guid.NewGuid():N}",
                Country = "Testland"
            })
        };

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCity_WithValidData_ShouldReturnCreated()
    {
        var token = await LoginAsAdminAsync();
        var request = ApiTestFixture.BuildCityRequest();
        using var httpRequest = ApiTestFixture.CreateAuthorizedRequest(HttpMethod.Post, 
            Router.CityRoutes.Create, token, request);

        var response = await _client.SendAsync(httpRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CityResponse>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("CITY_CREATED", body!.Code);
        Assert.NotNull(body.Data);
        Assert.Equal(request.Name, body.Data!.Name);
        Assert.Equal(request.Country, body.Data.Country);
        Assert.False(body.Data.CityId == Guid.Empty);
        Assert.NotEmpty(body.Data.RowVersion);
    }

    [Fact]
    public async Task CreateCity_WithDuplicateNameAndCountry_ShouldReturnValidationError()
    {
        var token = await LoginAsAdminAsync();
        var request = ApiTestFixture.BuildCityRequest();

        using var firstRequest = ApiTestFixture.CreateAuthorizedRequest(HttpMethod.Post, 
            Router.CityRoutes.Create, token, request);
        var firstResponse = await _client.SendAsync(firstRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        using var duplicateRequest = ApiTestFixture.CreateAuthorizedRequest(HttpMethod.Post, 
            Router.CityRoutes.Create, token, request);
        var response = await _client.SendAsync(duplicateRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("name", body.Errors!.Fields.Keys);
    }

    [Fact]
    public async Task CityCrud_WithValidData_ShouldGetUpdateAndDeleteCity()
    {
        var token = await LoginAsAdminAsync();
        var createRequest = ApiTestFixture.BuildCityRequest();

        using var createHttpRequest = ApiTestFixture.CreateAuthorizedRequest(HttpMethod.Post, 
            Router.CityRoutes.Create, token, createRequest);
        var createResponse = await _client.SendAsync(createHttpRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<CityResponse>>();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(created?.Data);
        var city = created!.Data!;

        using var getRequest = ApiTestFixture.CreateAuthorizedRequest(
            HttpMethod.Get, Router.CityRoutes.GetById.Replace("{id:guid}", city.CityId.ToString()), token);
        var getResponse = await _client.SendAsync(getRequest);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ApiResponse<CityResponse>>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(fetched?.Data);
        Assert.Equal(city.CityId, fetched!.Data!.CityId);

        var updateRequest = new
        {
            Name = $"{createRequest.Name} Updated",
            Country = createRequest.Country,
            PostOffice = "12345",
            RowVersion = city.RowVersion
        };
        using var updateHttpRequest = ApiTestFixture.CreateAuthorizedRequest(
            HttpMethod.Put,
            Router.CityRoutes.Update.Replace("{id:guid}", city.CityId.ToString()),
            token,
            updateRequest);
        var updateResponse = await _client.SendAsync(updateHttpRequest);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<CityResponse>>();

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updated?.Data);
        Assert.Equal("CITY_UPDATED", updated!.Code);
        Assert.Equal(updateRequest.Name, updated.Data!.Name);

        using var deleteRequest = ApiTestFixture.CreateAuthorizedRequest(
            HttpMethod.Delete, Router.CityRoutes.Delete.Replace("{id:guid}", city.CityId.ToString()), token);
        var deleteResponse = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        using var deletedGetRequest = ApiTestFixture.CreateAuthorizedRequest(
            HttpMethod.Get, Router.CityRoutes.GetById.Replace("{id:guid}", city.CityId.ToString()), token);
        var deletedGetResponse = await _client.SendAsync(deletedGetRequest);
        var deletedBody = await deletedGetResponse.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, deletedGetResponse.StatusCode);
        Assert.NotNull(deletedBody);
        Assert.Equal("CITY_NOT_FOUND", deletedBody!.Code);
    }

    [Fact]
    public async Task GetCityById_WithUnknownId_ShouldReturnNotFound()
    {
        var token = await LoginAsAdminAsync();
        var endpoint = Router.CityRoutes.GetById.Replace("{id:guid}", Guid.NewGuid().ToString());
        using var request = ApiTestFixture.CreateAuthorizedRequest(HttpMethod.Get, endpoint, token);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("CITY_NOT_FOUND", body!.Code);
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
