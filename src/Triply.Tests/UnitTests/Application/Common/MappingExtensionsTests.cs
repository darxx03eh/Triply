using Triply.Application.DTOs.Authentications;
using Triply.Application.Extensions;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Images;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Application.Common;

public class MappingExtensionsTests
{
    [Fact]
    public void ToCityResponse_MapsAllFieldsAndHotelsCount()
    {
        var city = TestData.City();
        city.ThumbnailUrl = "https://cdn.test/city.png";

        var response = city.ToCityResponse(hotelsCount: 3);

        Assert.Equal(city.CityId, response.CityId);
        Assert.Equal("Nablus", response.Name);
        Assert.Equal("Palestine", response.Country);
        Assert.Equal("P400", response.PostOffice);
        Assert.Equal(3, response.HotelsCount);
        Assert.Equal("https://cdn.test/city.png", response.ThumbnailUrl);
        Assert.Equal(TestData.RowVersion, response.RowVersion);
    }

    [Fact]
    public void ToHotelResponse_MapsFieldsImagesAndSortedAmenities()
    {
        var hotel = TestData.Hotel(TestData.City());
        hotel.HotelAmenities.Add(new HotelAmenities { Amenity = TestData.Amenity("Spa") });
        hotel.HotelAmenities.Add(new HotelAmenities { Amenity = TestData.Amenity("Free WiFi") });

        var response = hotel.ToHotelResponse("Nablus", ["a.png", "b.png"]);

        Assert.Equal(hotel.HotelId, response.HotelId);
        Assert.Equal("Nablus", response.CityName);
        Assert.Equal(hotel.HotelType, response.HotelType);
        Assert.Equal(hotel.Address, response.Address);
        Assert.Equal(["a.png", "b.png"], response.ImageUrls);
        Assert.Equal(["Free WiFi", "Spa"], response.Amenities);
    }

    [Fact]
    public void ToHotelSummaryResponse_MapsOwnerRoomsThumbnailAndDeletedFlag()
    {
        var owner = TestData.User();
        var hotel = TestData.Hotel(TestData.City(), owner: owner, isDeleted: true);

        var response = hotel.ToHotelSummaryResponse("Nablus", roomsCount: 7, thumbnailUrl: "thumb.png");

        Assert.Equal("Mahmoud Darawsheh", response.OwnerName);
        Assert.Equal(7, response.RoomsCount);
        Assert.Equal("thumb.png", response.ThumbnailUrl);
        Assert.True(response.IsDeleted);
    }

    [Fact]
    public void ToHotelSummaryResponse_WithoutOwner_HasNullOwnerName()
    {
        var response = TestData.Hotel(TestData.City()).ToHotelSummaryResponse("Nablus");

        Assert.Null(response.OwnerName);
        Assert.Equal(0, response.RoomsCount);
        Assert.Null(response.ThumbnailUrl);
    }

    [Fact]
    public void ToHotelImageResponse_MapsStatus()
    {
        var image = TestData.Image(TestData.Hotel(TestData.City()), order: 2, status: ImageStatus.Pending);

        var response = image.ToHotelImageResponse();

        Assert.Equal(2, response.DisplayOrder);
        Assert.Equal(ImageStatus.Pending, response.Status);
        Assert.Null(response.Url);
    }

    [Fact]
    public void ToRoomResponse_MapsAllFields()
    {
        var room = TestData.Room(TestData.Hotel(TestData.City()), number: "305", price: 159.5m);

        var response = room.ToRoomResponse("Red Sea", false, []);

        Assert.Equal("Red Sea", response.HotelName);
        Assert.Equal("305", response.Number);
        Assert.Equal(159.5m, response.PricePerNight);
        Assert.Equal(room.RoomType, response.RoomType);
        Assert.Equal(room.AdultCapacity, response.AdultCapacity);
        Assert.Equal(room.IsAvailable, response.IsAvailable);
    }

    [Fact]
    public void ToAmenityResponse_MapsFields()
    {
        var amenity = TestData.Amenity("Spa");

        var response = amenity.ToAmenityResponse();

        Assert.Equal(amenity.AmenityId, response.AmenityId);
        Assert.Equal("Spa", response.Name);
    }

    [Fact]
    public void ToTriplyUser_MapsRegisterRequest()
    {
        var request = new RegisterUserRequest
        {
            FirstName = "Mahmoud",
            LastName = "Darawsheh",
            Email = "m@triply.com",
            Username = "mahmoud",
            PhoneNumber = "+970591234567",
            DateOfBirth = new DateTime(2003, 2, 18)
        };

        var user = request.ToTriplyUser();

        Assert.Equal("mahmoud", user.UserName);
        Assert.Equal("m@triply.com", user.Email);
        Assert.Equal("+970591234567", user.PhoneNumber);
        Assert.Equal(new DateTime(2003, 2, 18), user.DateOfBirth);
    }

    [Fact]
    public void ToLoginApiResponse_DropsRefreshToken()
    {
        var response = new LoginResponse("Mahmoud Darawsheh", "access", "refresh").ToLoginApiResponse();

        Assert.Equal(new LoginApiResponse("Mahmoud Darawsheh", "access"), response);
    }
}
