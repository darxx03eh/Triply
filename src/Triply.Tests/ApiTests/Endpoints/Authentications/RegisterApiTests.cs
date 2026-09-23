using System.Net;
using System.Net.Http.Json;
using Triply.Api.Responses;
using Triply.Application.DTOs.Authentications;
using Triply.Domain.Results;
using Triply.Infrastructure.Routes;

namespace Triply.Tests.ApiTests.Endpoints.Authentications;

[Collection("Authentication API")]
[Trait("collection", "Authentication API")]
public class RegisterApiTests : IClassFixture<ApiTestFixture>
{
    private const string RegisterEndpoint = Router.AuthenticationRoutes.Register;
    private readonly HttpClient _client;
    public RegisterApiTests(ApiTestFixture fixture) => _client = fixture.Client;

    [Fact]
    public async Task Register_WithValidData_ShouldReturnsCreated()
    {
        var request = ApiTestFixture.BuildValidRegistrationRequest();
        
        var response = await _client.PostAsJsonAsync(Router.AuthenticationRoutes.Register, request);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<RegisterUserResponse>>();
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(body!.Data);
        Assert.Equal(body.Code, ResultResponseMessages.Authentication.Api.RegisterSucceeded.Code);
        Assert.False(string.IsNullOrEmpty(body.Message));
        Assert.False(string.IsNullOrEmpty(body.Data.Email));
        Assert.False(string.IsNullOrEmpty(body.Data.Username));
        Assert.False(string.IsNullOrEmpty(body.Data.Id.ToString()));
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnsValidationError()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var request = ApiTestFixture.BuildValidRegistrationRequest(suffix);

        var firstResponse = await _client.PostAsJsonAsync(Router.AuthenticationRoutes.Register, request);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var newSuffix = Guid.NewGuid().ToString("N")[..8];
        var duplicateRequest = new
        {
            Username = $"test_user_{newSuffix}",
            Email = $"test_user_{suffix}@example.com",
            Password = "StrongPassword123!",
            ConfirmPassword = "StrongPassword123!",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = $"+972568{Random.Shared.Next(100, 1000)}540",
            DateOfBirth = new DateTime(2003, 2, 18)
        };
        
        var response = await _client.PostAsJsonAsync(Router.AuthenticationRoutes.Register, duplicateRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<RegisterUserResponse>>();
        
        Assert.NotNull(body);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("VALIDATION_ERROR", body.Code);
        Assert.Null(body!.Data);
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(body!.Errors);
        Assert.NotNull(body!.Errors.Fields);
    }

    [Fact]
    public async Task Register_WithInvalidPayload_ShouldReturnValidationError()
    {
        var response = await _client.PostAsJsonAsync(Router.AuthenticationRoutes.Register, new
        {
            FirstName = "",
            LastName = "x",
            Email = "not-an-email",
            Username = "x@",
            Password = "short",
            ConfirmPassword = "different",
            PhoneNumber = "invalid",
            DateOfBirth = DateTime.UtcNow.AddYears(-10)
        });
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body!.Code);
        Assert.Null(body.Data);
        Assert.NotNull(body.Errors);
        Assert.NotEmpty(body.Errors!.Fields);
    }
}