using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Triply.Api.Responses;
using Triply.Infrastructure.Routes;
using Triply.Tests.ApiTests;
using Triply.Tests.IntegrationTests.Infrastructure;

namespace Triply.Tests.IntegrationTests.Endpoints.Authentications;

[Collection("Authentication Integration")]
[Trait("collection", "Authentication Integration")]
[Trait("Category", "IntegrationTests")]
public class EmailConfirmationApiTests : IClassFixture<TriplyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EmailConfirmationApiTests(TriplyWebApplicationFactory factory)
        => _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

    [Fact]
    public async Task ConfirmEmail_WithMissingQueryValues_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync(Router.AuthenticationRoutes.EmailConfirmation);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
