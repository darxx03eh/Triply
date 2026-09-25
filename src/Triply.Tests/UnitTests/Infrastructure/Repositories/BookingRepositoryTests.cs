using Sieve.Models;
using Triply.Domain.Enums.Bookings;
using Triply.Infrastructure.Repositories;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Database;

namespace Triply.Tests.UnitTests.Infrastructure.Repositories;

public class BookingRepositoryTests : IDisposable
{
    private readonly Triply.Infrastructure.Db.TriplyDbContext _db = TestDbContextFactory.Create();
    private readonly BookingRepository _repository;
    private readonly Triply.Domain.Entities.Room _room;
    private readonly Triply.Domain.Entities.Identity.TriplyUser _user;

    public BookingRepositoryTests()
    {
        _repository = new BookingRepository(_db, TestDbContextFactory.CreateSieveProcessor());
        var city = TestData.City();
        var hotel = TestData.Hotel(city);
        _room = TestData.Room(hotel);
        _user = TestData.User();
        _db.AddRange(city, hotel, _room, _user);
        _db.SaveChanges();
    }

    [Fact]
    public async Task IsRoomBookedAsync_MatchesPendingAndConfirmedOverlapsOnly()
    {
        _db.Bookings.AddRange(
            TestData.Booking(_room, _user, new DateOnly(2026, 10, 10), new DateOnly(2026, 10, 15), BookingStatus.Pending),
            TestData.Booking(_room, _user, new DateOnly(2026, 11, 1), new DateOnly(2026, 11, 5), BookingStatus.Cancelled));
        _db.SaveChanges();

        Assert.True(await _repository.IsRoomBookedAsync(_room.RoomId, new DateTime(2026, 10, 14), new DateTime(2026, 10, 18)));
        Assert.False(await _repository.IsRoomBookedAsync(_room.RoomId, new DateTime(2026, 10, 15), new DateTime(2026, 10, 18)));
        Assert.False(await _repository.IsRoomBookedAsync(_room.RoomId, new DateTime(2026, 11, 2), new DateTime(2026, 11, 3)));
    }

    [Fact]
    public async Task GetByConfirmationNumberAsync_LoadsPaymentAndRoomHotelCity()
    {
        const string confirmation = "TRP-TEST";
        var booking = TestData.Booking(_room, _user, new DateOnly(2026, 10, 10), new DateOnly(2026, 10, 15));
        booking.ConfirmationNumber = confirmation;
        booking.Payment = new Triply.Domain.Entities.Payment { BookingId = booking.BookingId, Provider = "mock", Amount = 100m };
        _db.Bookings.Add(booking);
        _db.SaveChanges();
        _db.ChangeTracker.Clear();

        var bookings = await _repository.GetByConfirmationNumberAsync(confirmation);

        var actual = Assert.Single(bookings);
        Assert.NotNull(actual.Payment);
        Assert.Equal("Nablus", actual.Room.Hotel.City.Name);
    }

    [Fact]
    public async Task GetUserBookingsAsync_ReturnsOnlyTheRequestedUsersBookingsAndTheTotal()
    {
        var otherUser = TestData.User("other", "other@triply.com");
        _db.Users.Add(otherUser);
        _db.Bookings.AddRange(
            TestData.Booking(_room, _user, new DateOnly(2026, 10, 10), new DateOnly(2026, 10, 12)),
            TestData.Booking(_room, otherUser, new DateOnly(2026, 10, 15), new DateOnly(2026, 10, 17)));
        _db.SaveChanges();

        var (bookings, total) = await _repository.GetUserBookingsAsync(_user.Id, new SieveModel { Page = 1, PageSize = 10 });

        Assert.Equal(1, total);
        Assert.Single(bookings);
        Assert.Equal(_user.Id, bookings[0].UserId);
    }

    [Fact]
    public async Task HasCompletedBookingAtHotelAsync_RequiresCompletedBookingAtTheHotel()
    {
        var otherHotel = TestData.Hotel(TestData.City("Ramallah"), "Other Hotel");
        var otherRoom = TestData.Room(otherHotel);
        _db.AddRange(otherHotel.City, otherHotel, otherRoom);
        _db.Bookings.AddRange(
            TestData.Booking(_room, _user, new DateOnly(2026, 10, 10), new DateOnly(2026, 10, 12), BookingStatus.Completed),
            TestData.Booking(otherRoom, _user, new DateOnly(2026, 10, 15), new DateOnly(2026, 10, 17), BookingStatus.Confirmed));
        _db.SaveChanges();

        Assert.True(await _repository.HasCompletedBookingAtHotelAsync(_user.Id, _room.HotelId));
        Assert.False(await _repository.HasCompletedBookingAtHotelAsync(_user.Id, otherHotel.HotelId));
    }

    public void Dispose() => _db.Dispose();
}
