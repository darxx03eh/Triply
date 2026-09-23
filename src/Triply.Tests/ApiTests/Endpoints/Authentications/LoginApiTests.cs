using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.DTOs.Authentications;
using Triply.Domain.Results;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Authentications;

[Collection("Authentication API")]
public class LoginApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public LoginApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Theory]
    [InlineData("admin@triply.com")]
    [InlineData("admin")]
    public async Task Login_WithValidIdentifier_ShouldReturnAccessToken(string identifier)
    {
        var response = await _client.PostAsJsonAsync(Router.AuthenticationRoutes.Login, new
        {
            Identifier = identifier,
            Password = "admin003+-"
        });
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LoginApiResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(ResultResponseMessages.Authentication.Api.LoginSucceeded.Code, body!.Code);
        Assert.NotNull(body.Data);
        Assert.Equal("Mahmoud Darawsheh", body.Data!.Name);
        Assert.False(string.IsNullOrWhiteSpace(body.Data.Access));
        Assert.Contains(response.Headers.GetValues("Set-Cookie"), 
            cookie => cookie.StartsWith("refresh="));
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        var response = await _client.PostAsJsonAsync(Router.AuthenticationRoutes.Login, new
        {
            Identifier = $"missing-{Guid.NewGuid():N}@example.com",
            Password = "wrong-password"
        });
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("INVALID_CREDENTIALS", body!.Code);
        Assert.Null(body.Data);
    }

    [Fact]
    public async Task Login_WithInvalidPayload_ShouldReturnValidationError()
    {
        var response = await _client.PostAsJsonAsync(Router.AuthenticationRoutes.Login, new
        {
            Identifier = "",
            Password = "short"
        });
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.NotNull(body.Errors);
        Assert.Contains("identifier", body.Errors!.Fields.Keys);
        Assert.Contains("password", body.Errors.Fields.Keys);
    }
}
