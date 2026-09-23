using System.Net.Http.Headers;
using System.Net.Http.Json;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Domain.Enums.Hotels;
using Triply.Domain.Enums.Rooms;

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
            PhoneNumber = $"+97256{Random.Shared.Next(1_000_000_000, 2_000_000_000)}",
            DateOfBirth = new DateTime(2003, 2, 18)
        };
    }

    public static CreateCityRequest BuildCityRequest()
        => new CreateCityRequest()
        {
            Name = $"Test City {Guid.NewGuid():N}",
            Country = "Testland",
            PostOffice = "12345",
        };

    public static CreateHotelRequest BuildHotelRequest(Guid cityId)
        => new()
        {
            Name = $"Test Hotel {Guid.NewGuid():N}",
            CityId = cityId,
            StarRating = 4,
            HotelType = HotelType.Boutique,
            Address = "1 Test Street",
            Description = "A hotel created by the API test suite.",
            Latitude = 30m + Random.Shared.Next(0, 5000) / 100m,
            Longitude = 30m + Random.Shared.Next(0, 10000) / 100m
        };

    public static CreateRoomRequest BuildRoomRequest(Guid hotelId)
        => new()
        {
            HotelId = hotelId,
            Number = $"R-{Guid.NewGuid():N}"[..3],
            RoomType = RoomType.Double,
            AdultCapacity = 2,
            ChildCapacity = 1,
            PricePerNight = 125.50m,
            IsAvailable = true,
            Description = "A room created by the API test suite."
        };

    public static CreateAmenityRequest BuildAmenityRequest()
        => new() { Name = $"Test Amenity {Guid.NewGuid():N}" };
    
    public static HttpRequestMessage CreateRequest(HttpMethod method, string endpoint,
        string? token = null, object? content = null)
    {
        var request = new HttpRequestMessage(method, endpoint);
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (content is not null)
            request.Content = JsonContent.Create(content);
        return request;
    }
    
    public void Dispose() => Client.Dispose();
}