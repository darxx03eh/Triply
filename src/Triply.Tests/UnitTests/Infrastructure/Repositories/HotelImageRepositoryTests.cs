using Triply.Domain.Entities;
using Triply.Domain.Enums.Images;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Database;

namespace Triply.Tests.UnitTests.Infrastructure.Repositories;

public class HotelImageRepositoryTests : IDisposable
{
    private readonly TriplyDbContext _db = TestDbContextFactory.Create();
    private readonly HotelImageRepository _repository;
    private readonly Hotel _hotel;

    public HotelImageRepositoryTests()
    {
        _repository = new HotelImageRepository(_db);
        var city = TestData.City();
        _hotel = TestData.Hotel(city);
        _db.AddRange(city, _hotel);
        _db.SaveChanges();
    }

    [Fact]
    public async Task GetNextDisplayOrderAsync_NoImages_ReturnsOne()
    {
        Assert.Equal(1, await _repository.GetNextDisplayOrderAsync(_hotel.HotelId));
    }

    [Fact]
    public async Task GetNextDisplayOrderAsync_ReturnsMaxPlusOneIncludingPendingAndFailed()
    {
        _db.HotelImages.AddRange(
            TestData.Image(_hotel, 1),
            TestData.Image(_hotel, 5, ImageStatus.Failed),
            TestData.Image(_hotel, 3, ImageStatus.Pending));
        _db.SaveChanges();

        Assert.Equal(6, await _repository.GetNextDisplayOrderAsync(_hotel.HotelId));
    }

    [Fact]
    public async Task GetByHotelIdAsync_ReturnsAllStatusesOrderedByDisplayOrder()
    {
        _db.HotelImages.AddRange(
            TestData.Image(_hotel, 2, ImageStatus.Pending),
            TestData.Image(_hotel, 1),
            TestData.Image(TestData.Hotel(TestData.City("Other")), 1));
        _db.SaveChanges();

        var images = await _repository.GetByHotelIdAsync(_hotel.HotelId);

        Assert.Equal([1, 2], images.Select(i => (int)i.DisplayOrder));
    }

    public void Dispose() => _db.Dispose();
}
