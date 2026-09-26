using Sieve.Models;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Bookings;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Database;

namespace Triply.Tests.UnitTests.Infrastructure.Repositories;

public class RoomRepositoryTests : IDisposable
{
    private readonly TriplyDbContext _db = TestDbContextFactory.Create();
    private readonly RoomRepository _repository;
    private readonly Hotel _hotel;
    private readonly Hotel _other;
    private readonly Room _room101;

    public RoomRepositoryTests()
    {
        _repository = new RoomRepository(_db, TestDbContextFactory.CreateSieveProcessor());
        var city = TestData.City();
        _hotel = TestData.Hotel(city, "Main");
        _other = TestData.Hotel(city, "Other");
        _room101 = TestData.Room(_hotel, "101", 100m);
        _db.AddRange(city, _hotel, _other);
        _db.Rooms.AddRange(
            _room101,
            TestData.Room(_hotel, "102", 50m, isAvailable: false),
            TestData.Room(_hotel, "103", 80m, isDeleted: true),
            TestData.Room(_other, "101", 70m));
        _db.SaveChanges();
    }

    [Theory]
    [InlineData("101", true)]
    [InlineData("  ", false)]
    [InlineData("999", false)]
    [InlineData("103", false)]
    public async Task IsRoomNumberExistsAsync_ChecksWithinHotelIgnoringDeleted(string number, bool expected)
    {
        Assert.Equal(expected, await _repository.IsRoomNumberExistsAsync(_hotel.HotelId, number));
    }

    [Fact]
    public async Task IsRoomNumberExistsAsync_IsCaseInsensitive()
    {
        _db.Rooms.Add(TestData.Room(_hotel, "T-1"));
        _db.SaveChanges();

        Assert.True(await _repository.IsRoomNumberExistsAsync(_hotel.HotelId, "t-1"));
    }

    [Fact]
    public async Task IsRoomNumberExistsExcludeIdAsync_OnlyChecksSameHotelAndSkipsSelf()
    {
        Assert.False(await _repository.IsRoomNumberExistsExcludeIdAsync("101", _room101.RoomId));
        Assert.True(await _repository.IsRoomNumberExistsExcludeIdAsync("102", _room101.RoomId));

        var otherRoom = _db.Rooms.Single(r => r.HotelId == _other.HotelId);
        Assert.False(await _repository.IsRoomNumberExistsExcludeIdAsync("102", otherRoom.RoomId));
    }

    [Fact]
    public async Task GetByIdWithHotelAsync_LoadsHotel()
    {
        _db.ChangeTracker.Clear();

        var room = await _repository.GetByIdWithHotelAndImagesAsync(_room101.RoomId);

        Assert.Equal("Main", room!.Hotel.Name);
    }

    [Theory]
    [InlineData(false, 3)]
    [InlineData(true, 4)]
    public async Task GetPagedAsync_AdminAlsoSeesDeletedRooms(bool isAdmin, int expected)
    {
        var (_, total) = await _repository.GetPagedAsync(new SieveModel(), isAdmin);

        Assert.Equal(expected, total);
    }

    [Fact]
    public async Task GetPagedAsync_HotelIdFilter_ReturnsOnlyThatHotel()
    {
        var (rooms, total) = await _repository.GetPagedAsync(new SieveModel(), 
            false, _hotel.HotelId);

        Assert.Equal(2, total);
        Assert.All(rooms, r => Assert.Equal(_hotel.HotelId, r.HotelId));
    }

    [Fact]
    public async Task GetPagedAsync_SieveAliases_FilterAndSort()
    {
        var (rooms, _) = await _repository.GetPagedAsync(
            new SieveModel { Filters = "available==true", Sorts = "-price" }, false);

        Assert.Equal([100m, 70m], rooms.Select(r => r.PricePerNight));
    }

    [Fact]
    public async Task IsBookedAsync_ReturnsTrueOnlyForOverlappingPendingOrConfirmedBookings()
    {
        var user = TestData.User();
        _db.Users.Add(user);
        _db.Bookings.AddRange(
            TestData.Booking(_room101, user, new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 3), BookingStatus.Cancelled),
            TestData.Booking(_room101, user, new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 7), BookingStatus.Confirmed));
        _db.SaveChanges();

        Assert.True(await _repository.IsBookedAsync(_room101.RoomId,
            new DateTime(2026, 10, 6), new DateTime(2026, 10, 8)));
        Assert.False(await _repository.IsBookedAsync(_room101.RoomId,
            new DateTime(2026, 10, 7), new DateTime(2026, 10, 9)));
        Assert.False(await _repository.IsBookedAsync(_room101.RoomId,
            new DateTime(2026, 10, 3), new DateTime(2026, 10, 5)));

        var otherRoom = _db.Rooms.Single(room => room.HotelId == _other.HotelId);
        Assert.False(await _repository.IsBookedAsync(otherRoom.RoomId,
            new DateTime(2026, 10, 6), new DateTime(2026, 10, 8)));
    }

    public void Dispose() => _db.Dispose();
}
