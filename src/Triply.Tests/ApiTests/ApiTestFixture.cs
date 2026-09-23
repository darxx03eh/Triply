namespace Triply.Tests.ApiTests;

public class ApiTestFixture : IDisposable
{
    public HttpClient Client { get; }

    public ApiTestFixture()
    {
        var baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
                      ?? "http://localhost:8080";
        Client = new HttpClient(new HttpClientHandler { UseCookies = true })
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    public static object BuildValidRegistrationRequest(string? uniqueSuffix = null)
    {
        var suffix = uniqueSuffix ?? Guid.NewGuid().ToString("N")[..8];
        return new
        {
            Username = $"test_user_{suffix}",
            Email = $"test_user_{suffix}@example.com",
            Password = "StrongPassword123!",
            ConfirmPassword = "StrongPassword123!",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = $"+972568{Random.Shared.Next(100, 1000)}540",
            DateOfBirth = new DateTime(2003, 2, 18)
        };
    }

    public void Dispose() => Client.Dispose();
}