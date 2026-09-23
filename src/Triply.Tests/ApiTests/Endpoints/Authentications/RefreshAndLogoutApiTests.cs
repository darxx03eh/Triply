using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.DTOs.Authentications;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Authentications;

[Collection("Auth")]
[Trait("collection", "Authentication API")]
[Trait("Category", "ApiTests")]
public class RefreshAndLogoutApiTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public RefreshAndLogoutApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task Refresh_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Router.AuthenticationRoutes.Refresh);
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithValidLogin_ShouldReturnNewAccessToken()
    {
        var (accessToken, refreshCookie) = await LoginAsync();
        using var request = new HttpRequestMessage(HttpMethod.Post, Router.AuthenticationRoutes.Refresh);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("Cookie", refreshCookie);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LoginApiResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("TOKEN_REGENERATED", body!.Code);
        Assert.NotNull(body.Data);
        Assert.False(string.IsNullOrWhiteSpace(body.Data!.Access));
        Assert.NotEqual(accessToken, body.Data.Access);
    }

    [Fact]
    public async Task Logout_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, Router.AuthenticationRoutes.Logout);
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithValidLogin_ShouldReturnNoContentAndClearRefreshCookie()
    {
        var (accessToken, refreshCookie) = await LoginAsync();
        using var request = new HttpRequestMessage(HttpMethod.Delete, Router.AuthenticationRoutes.Logout);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("Cookie", refreshCookie);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Contains(response.Headers.GetValues("Set-Cookie"),
            cookie => cookie.Contains("refresh=", StringComparison.OrdinalIgnoreCase)
                      && cookie.Contains("expires=Thu, 01 Jan 1970 00:00:00 GMT",
                          StringComparison.OrdinalIgnoreCase));
    }

    private async Task<(string AccessToken, string RefreshCookie)> LoginAsync()
    {
        var response = await _client.PostAsJsonAsync(Router.AuthenticationRoutes.Login, new
        {
            Identifier = "admin@triply.com",
            Password = "admin003+-"
        });
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LoginApiResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body?.Data);
        var setCookie = Assert.Single(response.Headers.GetValues("Set-Cookie"));
        var refreshCookie = setCookie[..setCookie.IndexOf(';')];
        return (body!.Data!.Access, refreshCookie);
    }
}
