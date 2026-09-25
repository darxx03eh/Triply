using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Database;

namespace Triply.Tests.UnitTests.Infrastructure.Repositories;

public class AmenityRepositoryTests : IDisposable
{
    private readonly TriplyDbContext _db = TestDbContextFactory.Create();
    private readonly AmenityRepository _repository;
    private readonly Hotel _hotel;
    private readonly Amenity _spa = TestData.Amenity("Spa");
    private readonly Amenity _pool = TestData.Amenity("Swimming Pool");
    private readonly Amenity _wifi = TestData.Amenity("Free WiFi");

    public AmenityRepositoryTests()
    {
        _repository = new AmenityRepository(_db);
        var city = TestData.City();
        _hotel = TestData.Hotel(city);
        _db.AddRange(city, _hotel, _spa, _pool, _wifi);
        _db.SaveChanges();
    }

    private List<Guid> CurrentIds()
        => _db.HotelAmenities.Where(ha => 
            ha.HotelId == _hotel.HotelId).Select(ha => 
            ha.AmenityId).OrderBy(id => id).ToList();

    [Fact]
    public async Task GetAllOrderedAsync_ReturnsAlphabetical()
    {
        var names = (await _repository.GetAllOrderedAsync()).Select(a => a.Name);

        Assert.Equal(["Free WiFi", "Spa", "Swimming Pool"], names);
    }

    [Theory]
    [InlineData("Spa", true)]
    [InlineData("SPA", true)]
    [InlineData("Sauna", false)]
    public async Task IsAmenityExistsAsync_IgnoresCase(string name, bool expected)
    {
        Assert.Equal(expected, await _repository.IsAmenityExistsAsync(name));
    }

    [Fact]
    public async Task IsAmenityExistsExcludeIdAsync_IgnoresItself()
    {
        Assert.False(await _repository.IsAmenityExistsExcludeIdAsync("Spa", _spa.AmenityId));
        Assert.True(await _repository.IsAmenityExistsExcludeIdAsync("Spa", _pool.AmenityId));
    }

    [Fact]
    public async Task CountExistingAsync_CountsDistinctKnownIds()
    {
        Assert.Equal(2, await _repository.CountExistingAsync(
            [_spa.AmenityId, _spa.AmenityId, _pool.AmenityId, Guid.NewGuid()]));
    }

    [Fact]
    public async Task ReplaceHotelAmenitiesAsync_AddsNewRemovesMissingKeepsExisting()
    {
        _db.HotelAmenities.AddRange(
            new HotelAmenities { HotelId = _hotel.HotelId, AmenityId = _spa.AmenityId },
            new HotelAmenities { HotelId = _hotel.HotelId, AmenityId = _pool.AmenityId });
        _db.SaveChanges();
        var keptLinkId = _db.HotelAmenities.Single(ha => ha.AmenityId == _spa.AmenityId).HotelAmenitiesId;

        await _repository.ReplaceHotelAmenitiesAsync(_hotel.HotelId, [_spa.AmenityId, _wifi.AmenityId]);
        await _repository.SaveChangesAsync(default);

        Assert.Equal(new[] { _spa.AmenityId, _wifi.AmenityId }.OrderBy(id => id), CurrentIds());
        Assert.Equal(keptLinkId, _db.HotelAmenities.Single(ha => ha.AmenityId == _spa.AmenityId).HotelAmenitiesId);
    }

    [Fact]
    public async Task ReplaceHotelAmenitiesAsync_EmptyList_RemovesAll()
    {
        _db.HotelAmenities.Add(new HotelAmenities { HotelId = _hotel.HotelId, AmenityId = _spa.AmenityId });
        _db.SaveChanges();

        await _repository.ReplaceHotelAmenitiesAsync(_hotel.HotelId, []);
        await _repository.SaveChangesAsync(default);

        Assert.Empty(CurrentIds());
    }

    [Fact]
    public async Task GetByHotelIdAsync_ReturnsHotelAmenitiesAlphabetical()
    {
        _db.HotelAmenities.AddRange(
            new HotelAmenities { HotelId = _hotel.HotelId, AmenityId = _spa.AmenityId },
            new HotelAmenities { HotelId = _hotel.HotelId, AmenityId = _wifi.AmenityId });
        _db.SaveChanges();

        var names = (await _repository.GetByHotelIdAsync(_hotel.HotelId)).Select(a => a.Name);

        Assert.Equal(["Free WiFi", "Spa"], names);
    }

    public void Dispose() => _db.Dispose();
}
