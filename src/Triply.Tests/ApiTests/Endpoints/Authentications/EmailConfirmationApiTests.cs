using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Authentications;

[Collection("Authentication API")]
public class EmailConfirmationApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public EmailConfirmationApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task ConfirmEmail_WithMissingQueryValues_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync(Router.AuthenticationRoutes.EmailConfirmation);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("INVALID_REQUEST_BODY", body!.Code);
        Assert.Null(body.Errors);
    }

    [Fact]
    public async Task ConfirmEmail_ForAlreadyConfirmedUser_ShouldReturnBusinessError()
    {
        var endpoint = $"{Router.AuthenticationRoutes.EmailConfirmation}?email=admin%40triply.com&token=unused";
        var response = await _client.GetAsync(endpoint);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("EMAIL_ALREADY_VERIFIED", body!.Code);
        Assert.Null(body.Data);
    }
}
