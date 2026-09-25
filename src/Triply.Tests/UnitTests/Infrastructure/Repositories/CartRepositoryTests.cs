using Triply.Domain.Entities;
using Triply.Infrastructure.Repositories;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Database;

namespace Triply.Tests.UnitTests.Infrastructure.Repositories;

public class CartRepositoryTests : IDisposable
{
    private readonly Triply.Infrastructure.Db.TriplyDbContext _db = TestDbContextFactory.Create();
    private readonly CartRepository _repository;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Room _room;

    public CartRepositoryTests()
    {
        _repository = new CartRepository(_db);
        var city = TestData.City();
        var hotel = TestData.Hotel(city);
        _room = TestData.Room(hotel);
        _db.AddRange(city, hotel, _room);
        _db.SaveChanges();
    }

    [Fact]
    public async Task IsItemExistsAsync_ReturnsTrueOnlyForOverlappingDatesOfTheSameRoomAndUser()
    {
        _db.CartItems.Add(new CartItem
        {
            UserId = _userId, RoomId = _room.RoomId,
            CheckIn = new DateTime(2026, 10, 10), CheckOut = new DateTime(2026, 10, 15)
        });
        _db.SaveChanges();

        Assert.True(await _repository.IsItemExistsAsync(_userId, _room.RoomId,
            new DateTime(2026, 10, 14), new DateTime(2026, 10, 18)));
        Assert.False(await _repository.IsItemExistsAsync(_userId, _room.RoomId,
            new DateTime(2026, 10, 15), new DateTime(2026, 10, 18)));
        Assert.False(await _repository.IsItemExistsAsync(Guid.NewGuid(), _room.RoomId,
            new DateTime(2026, 10, 14), new DateTime(2026, 10, 18)));
    }

    [Fact]
    public async Task GetUserCartAsync_ReturnsOnlyTheUsersItemsAndLoadsRoomHotelAndCity()
    {
        _db.CartItems.AddRange(
            new CartItem { UserId = _userId, RoomId = _room.RoomId, CheckIn = DateTime.UtcNow, CheckOut = DateTime.UtcNow.AddDays(1) },
            new CartItem { UserId = Guid.NewGuid(), RoomId = _room.RoomId, CheckIn = DateTime.UtcNow, CheckOut = DateTime.UtcNow.AddDays(1) });
        _db.SaveChanges();
        _db.ChangeTracker.Clear();

        var cart = await _repository.GetUserCartAsync(_userId);

        var item = Assert.Single(cart);
        Assert.Equal(_room.RoomId, item.RoomId);
        Assert.NotNull(item.Room.Hotel.City);
    }

    public void Dispose() => _db.Dispose();
}
