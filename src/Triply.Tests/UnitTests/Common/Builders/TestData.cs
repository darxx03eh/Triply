using Triply.Domain.Entities;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Enums.Hotels;
using Triply.Domain.Enums.Images;
using Triply.Domain.Enums.Rooms;

namespace Triply.Tests.UnitTests.Common.Builders;

/// <summary>Factory methods for valid domain entities; override only what a test cares about.</summary>
public static class TestData
{
    public static readonly byte[] RowVersion = [0, 0, 0, 0, 0, 0, 0, 1];

    public static City City(string name = "Nablus", string country = "Palestine", bool isDeleted = false)
        => new()
        {
            Name = name,
            Country = country,
            PostOffice = "P400",
            IsDeleted = isDeleted,
            RowVersion = RowVersion
        };

    public static Hotel Hotel(City city, string name = "Darawsheh Hotel", byte stars = 5,
        HotelType type = HotelType.Luxury, bool isDeleted = false, TriplyUser? owner = null)
        => new()
        {
            Name = name,
            City = city,
            CityId = city.CityId,
            StarRating = stars,
            HotelType = type,
            Address = "Rafidia Street 100",
            Description = "A nice hotel",
            Latitude = 32.22m,
            Longitude = 35.25m,
            IsDeleted = isDeleted,
            Owner = owner,
            OwnerId = owner?.Id,
            RowVersion = RowVersion
        };

    public static Room Room(Hotel hotel, string number = "101", decimal price = 100m, RoomType type = RoomType.Double,
        short adults = 2, short children = 0, bool isAvailable = true, bool isDeleted = false)
        => new()
        {
            Hotel = hotel,
            HotelId = hotel.HotelId,
            Number = number,
            PricePerNight = price,
            RoomType = type,
            AdultCapacity = adults,
            ChildCapacity = children,
            IsAvailable = isAvailable,
            IsDeleted = isDeleted,
            Description = "Room description",
            RowVersion = RowVersion
        };

    public static Amenity Amenity(string name = "Free WiFi") => new() { Name = name };

    public static HotelImage Image(Hotel hotel, short order = 1, ImageStatus status = ImageStatus.Uploaded,
        string? url = "https://cdn.test/image.png", string? publicId = "triply/hotels/image")
        => new()
        {
            Hotel = hotel,
            HotelId = hotel.HotelId,
            DisplayOrder = order,
            Status = status,
            Url = status == ImageStatus.Uploaded ? url : null,
            PublicId = status == ImageStatus.Uploaded ? publicId : null
        };

    public static TriplyUser User(string username = "mahmoud", string email = "mahmoud@triply.com",
        bool emailConfirmed = true, bool isActive = true)
        => new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Mahmoud",
            LastName = "Darawsheh",
            UserName = username,
            Email = email,
            PhoneNumber = "+970591234567",
            DateOfBirth = new DateTime(2000, 1, 1),
            EmailConfirmed = emailConfirmed,
            IsActive = isActive
        };

    public static Booking Booking(Room room, TriplyUser user, DateOnly checkIn, DateOnly checkOut,
        BookingStatus status = BookingStatus.Confirmed)
        => new()
        {
            Room = room,
            RoomId = room.RoomId,
            User = user,
            UserId = user.Id,
            CheckIn = checkIn.ToDateTime(TimeOnly.MinValue),
            CheckOut = checkOut.ToDateTime(TimeOnly.MinValue),
            Adults = 1,
            ConfirmationNumber = $"TRP-{Guid.NewGuid():N}",
            GuestFullName = "Test Guest",
            GuestEmail = "guest@triply.com",
            TotalPrice = room.PricePerNight,
            Status = status
        };
}
