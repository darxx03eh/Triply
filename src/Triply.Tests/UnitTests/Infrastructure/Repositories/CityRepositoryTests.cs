using Sieve.Models;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Database;

namespace Triply.Tests.UnitTests.Infrastructure.Repositories;

public class CityRepositoryTests : IDisposable
{
    private readonly TriplyDbContext _db = TestDbContextFactory.Create();
    private readonly CityRepository _repository;
    private readonly City _nablus = TestData.City("Nablus", "Palestine");
    private readonly City _amman = TestData.City("Amman", "Jordan");
    private readonly City _deleted = TestData.City("Gone", "Nowhere", isDeleted: true);

    public CityRepositoryTests()
    {
        _repository = new CityRepository(_db, TestDbContextFactory.CreateSieveProcessor());
        _db.AddRange(_nablus, _amman, _deleted);
        _db.SaveChanges();
    }

    [Theory]
    [InlineData("Nablus", "Palestine", true)]
    [InlineData("NABLUS", "palestine", true)]
    [InlineData("Nablus", "Jordan", false)]
    [InlineData("Gone", "Nowhere", false)]
    public async Task IsCityExistsAsync_MatchesNameAndCountryIgnoringCase(string name, string country, bool expected)
    {
        Assert.Equal(expected, await _repository.IsCityExistsAsync(name, country));
    }

    [Fact]
    public async Task IsCityExistsExcludeId_IgnoresTheCityItself()
    {
        Assert.False(await _repository.IsCityExistsExcludeId("Nablus", "Palestine", _nablus.CityId));
        Assert.True(await _repository.IsCityExistsExcludeId("Nablus", "Palestine", _amman.CityId));
    }

    [Fact]
    public async Task IsCityIdExistsAsync_IgnoresDeletedCities()
    {
        Assert.True(await _repository.IsCityIdExistsAsync(_amman.CityId));
        Assert.False(await _repository.IsCityIdExistsAsync(_deleted.CityId));
    }

    [Fact]
    public async Task GetHotelsCountAsync_CountsActiveHotelsPerCity()
    {
        _db.Hotels.AddRange(
            TestData.Hotel(_amman, "A"), TestData.Hotel(_amman, "B"), TestData.Hotel(_amman, "C", 
                isDeleted: true),
            TestData.Hotel(_nablus, "D"));
        _db.SaveChanges();

        var counts = await _repository.GetHotelsCountAsync([_amman.CityId, 
            _nablus.CityId, _deleted.CityId]);

        Assert.Equal(2, counts[_amman.CityId]);
        Assert.Equal(1, counts[_nablus.CityId]);
        Assert.False(counts.ContainsKey(_deleted.CityId));
    }

    [Theory]
    [InlineData(false, 2)]
    [InlineData(true, 3)]
    public async Task GetPagedAsync_AdminAlsoSeesDeletedCities(bool isAdmin, int expected)
    {
        var (_, total) = await _repository.GetPagedAsync(new SieveModel(), isAdmin);

        Assert.Equal(expected, total);
    }

    [Fact]
    public async Task GetPagedAsync_FiltersAndSortsWithSieve()
    {
        var (cities, _) = await _repository.GetPagedAsync(
            new SieveModel { Filters = "Country==Jordan" }, false);

        Assert.Equal("Amman", Assert.Single(cities).Name);
    }

    public void Dispose() => _db.Dispose();
}
