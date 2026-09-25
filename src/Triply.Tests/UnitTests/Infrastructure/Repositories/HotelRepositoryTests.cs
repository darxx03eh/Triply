using Sieve.Models;
using Triply.Domain.Entities;
using Triply.Domain.Enums.HotleImages;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Database;

namespace Triply.Tests.UnitTests.Infrastructure.Repositories;

public class HotelRepositoryTests : IDisposable
{
    private readonly TriplyDbContext _db = TestDbContextFactory.Create();
    private readonly HotelRepository _repository;
    private readonly City _city = TestData.City();
    private readonly Hotel _hotel;
    private readonly Hotel _deleted;

    public HotelRepositoryTests()
    {
        _repository = new HotelRepository(_db, TestDbContextFactory.CreateSieveProcessor());
        _hotel = TestData.Hotel(_city, "Darawsheh Hotel");
        _deleted = TestData.Hotel(_city, "Old Hotel", isDeleted: true);
        _deleted.Latitude = 1m;
        _deleted.Longitude = 1m;
        _db.AddRange(_city, _hotel, _deleted);
        _db.SaveChanges();
    }

    [Theory]
    [InlineData("Darawsheh Hotel", true)]
    [InlineData("DARAWSHEH HOTEL", true)]
    [InlineData("Another", false)]
    [InlineData("Old Hotel", false)]
    public async Task IsHotelExistsAsync_ChecksNameInCityIgnoringCaseAndDeleted(string name, bool expected)
    {
        Assert.Equal(expected, await _repository.IsHotelExistsAsync(name, _city.CityId, default));
    }

    [Fact]
    public async Task IsHotelExistsAsync_SameNameInOtherCity_IsFalse()
    {
        Assert.False(await _repository.IsHotelExistsAsync("Darawsheh Hotel", 
            Guid.NewGuid(), default));
    }

    [Fact]
    public async Task IsHotelExistsExcludeId_IgnoresTheHotelItself()
    {
        Assert.False(await _repository.IsHotelExistsExcludeId("Darawsheh Hotel", _city.CityId, _hotel.HotelId));
        Assert.True(await _repository.IsHotelExistsExcludeId("Darawsheh Hotel", _city.CityId, Guid.NewGuid()));
    }

    [Fact]
    public async Task LocationChecks_MatchCoordinatesAndExcludeSelf()
    {
        Assert.True(await _repository.IsLocationsExistsAsync(32.22m, 35.25m, default));
        Assert.False(await _repository.IsLocationsExistsAsync(1m, 1m, default));
        Assert.False(await _repository.IsLocationExistsExcludeIdAsync(32.22m, 35.25m, _hotel.HotelId));
        Assert.True(await _repository.IsLocationExistsExcludeIdAsync(32.22m, 35.25m, Guid.NewGuid()));
    }

    [Fact]
    public async Task IsHotelIdExistsAsync_IgnoresDeletedHotels()
    {
        Assert.True(await _repository.IsHotelIdExistsAsync(_hotel.HotelId));
        Assert.False(await _repository.IsHotelIdExistsAsync(_deleted.HotelId));
        Assert.False(await _repository.IsHotelIdExistsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetRoomsCountAsync_CountsOnlyActiveRoomsPerHotel()
    {
        _db.Rooms.AddRange(TestData.Room(_hotel, "1"), TestData.Room(_hotel, "2"), TestData.Room(_hotel, "3", isDeleted: true));
        _db.SaveChanges();

        var counts = await _repository.GetRoomsCountAsync([_hotel.HotelId, _deleted.HotelId]);

        Assert.Equal(2, counts[_hotel.HotelId]);
        Assert.False(counts.ContainsKey(_deleted.HotelId));
    }

    [Fact]
    public async Task GetThumbnailsAsync_ReturnsFirstUploadedImageByDisplayOrder()
    {
        _db.HotelImages.AddRange(
            TestData.Image(_hotel, 1, HotelImageStatus.Failed),
            TestData.Image(_hotel, 3, url: "third.png"),
            TestData.Image(_hotel, 2, url: "second.png"));
        _db.SaveChanges();

        var thumbnails = await _repository.GetThumbnailsAsync([_hotel.HotelId]);

        Assert.Equal("second.png", thumbnails[_hotel.HotelId]);
    }

    [Fact]
    public async Task GetByIdWithImagesAsync_LoadsCityUploadedImagesAndAmenities()
    {
        _db.HotelImages.AddRange(TestData.Image(_hotel, 1, HotelImageStatus.Pending), 
            TestData.Image(_hotel, 2, url: "ok.png"));
        _db.HotelAmenities.Add(new HotelAmenities { Hotel = _hotel, Amenity = TestData.Amenity("Spa") });
        _db.SaveChanges();
        _db.ChangeTracker.Clear();

        var hotel = await _repository.GetByIdWithImagesAsync(_hotel.HotelId);

        Assert.Equal("Nablus", hotel!.City.Name);
        Assert.Equal("ok.png", Assert.Single(hotel.Images).Url);
        Assert.Equal("Spa", Assert.Single(hotel.HotelAmenities).Amenity.Name);
    }

    [Fact]
    public async Task GetByIdWithImagesAsync_DeletedHotel_ReturnsNull()
    {
        Assert.Null(await _repository.GetByIdWithImagesAsync(_deleted.HotelId));
    }

    [Theory]
    [InlineData(false, 1)]
    [InlineData(true, 2)]
    public async Task GetPagedAsync_AdminAlsoSeesDeletedHotels(bool isAdmin, int expected)
    {
        var (hotels, total) = await _repository.GetPagedAsync(new SieveModel(), isAdmin);

        Assert.Equal(expected, total);
        Assert.Equal(expected, hotels.Count);
        Assert.All(hotels, h => Assert.NotNull(h.City));
    }

    [Fact]
    public async Task GetPagedAsync_AppliesSieveFilterAndPaging()
    {
        _db.Hotels.AddRange(TestData.Hotel(_city, "Alpha"), TestData.Hotel(_city, "Beta"));
        _db.SaveChanges();

        var (hotels, total) = await _repository.GetPagedAsync(
            new SieveModel { Filters = "Name@=a", Sorts = "Name", Page = 1, PageSize = 2 }, isAdmin: false);

        Assert.Equal(3, total);
        Assert.Equal(["Alpha", "Beta"], hotels.Select(h => h.Name));
    }

    [Fact]
    public async Task SoftDeleteRoomsAsync_MarksAllRoomsOfHotelOnSave()
    {
        var other = TestData.Hotel(_city, "Other");
        _db.AddRange(other, TestData.Room(_hotel, "1"), TestData.Room(_hotel, "2"), TestData.Room(other, "9"));
        _db.SaveChanges();

        await _repository.SoftDeleteRoomsAsync(_hotel.HotelId);
        await _repository.SaveChangesAsync(default);

        var rooms = _db.Rooms.IgnoreQueryFiltersAll().ToList();
        Assert.All(rooms.Where(r => r.HotelId == _hotel.HotelId), r => Assert.True(r.IsDeleted));
        Assert.False(rooms.Single(r => r.Number == "9").IsDeleted);
    }

    public void Dispose() => _db.Dispose();
}

internal static class QueryFilterExtensions
{
    public static IQueryable<T> IgnoreQueryFiltersAll<T>(this IQueryable<T> query) where T : class
        => Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.IgnoreQueryFilters(query);
}
