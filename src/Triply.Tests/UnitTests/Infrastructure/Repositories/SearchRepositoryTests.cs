using Triply.Application.Extensions;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Domain.Entities;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Enums.Hotels;
using Triply.Domain.Enums.Images;
using Triply.Domain.Enums.Rooms;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Database;

namespace Triply.Tests.UnitTests.Infrastructure.Repositories;

public class SearchRepositoryTests : IDisposable
{
    private readonly TriplyDbContext _db = TestDbContextFactory.Create();
    private readonly SearchRepository _repository;
    private readonly TriplyUser _guest = TestData.User();

    private readonly City _amman = TestData.City("Amman", "Jordan");
    private readonly City _paris = TestData.City("Paris", "France");
    private readonly Hotel _rotana;
    private readonly Hotel _budget;
    private readonly Hotel _boutique;
    private readonly Room _rotanaSingle;
    private readonly Room _rotanaSuite;
    private readonly Amenity _spa = TestData.Amenity("Spa");
    private readonly Amenity _pool = TestData.Amenity("Swimming Pool");
    private readonly Amenity _wifi = TestData.Amenity("Free WiFi");

    private static readonly DateOnly CheckIn = SearchExtensions.Today().AddDays(10);
    private static readonly DateOnly CheckOut = CheckIn.AddDays(3);

    public SearchRepositoryTests()
    {
        _repository = new SearchRepository(_db);

        _rotana = TestData.Hotel(_amman, "Amman Rotana", 5, HotelType.Luxury);
        _budget = TestData.Hotel(_amman, "Downtown Budget", 2, HotelType.Budget);
        _boutique = TestData.Hotel(_paris, "Le Marais Boutique", 4, HotelType.Boutique);

        _rotanaSingle = TestData.Room(_rotana, "101", 150m, RoomType.Single, adults: 1);
        _rotanaSuite = TestData.Room(_rotana, "501", 400m, RoomType.Suite, adults: 4, children: 2);
        var rotanaDouble = TestData.Room(_rotana, "201", 220m, RoomType.Double, adults: 2, children: 1);

        _db.AddRange(_amman, _paris, _rotana, _budget, _boutique, _guest, _spa, _pool, _wifi);
        _db.Rooms.AddRange(
            _rotanaSingle, _rotanaSuite, rotanaDouble,
            TestData.Room(_budget, "1", 35m, adults: 2),
            TestData.Room(_budget, "2", 30m, adults: 2, isAvailable: false),
            TestData.Room(_budget, "3", 25m, adults: 2, isDeleted: true),
            TestData.Room(_boutique, "10", 180m, adults: 2),
            TestData.Room(_boutique, "11", 190m, adults: 2));
        _db.HotelAmenities.AddRange(
            new HotelAmenities { Hotel = _rotana, Amenity = _spa },
            new HotelAmenities { Hotel = _rotana, Amenity = _pool },
            new HotelAmenities { Hotel = _rotana, Amenity = _wifi },
            new HotelAmenities { Hotel = _boutique, Amenity = _wifi },
            new HotelAmenities { Hotel = _boutique, Amenity = _spa });
        _db.HotelImages.AddRange(
            TestData.Image(_rotana, 2, url: "rotana-2.png"),
            TestData.Image(_rotana, 1, ImageStatus.Pending),
            TestData.Image(_rotana, 3, url: "rotana-3.png"));
        _db.SaveChanges();
    }

    private static SearchHotelsCriteria Criteria(Action<SearchHotelsRequest>? configure = null)
    {
        var request = new SearchHotelsRequest { CheckIn = CheckIn, CheckOut = CheckOut, PageSize = 50 };
        configure?.Invoke(request);
        return request.ToCriteria();
    }

    private async Task<List<string>> Names(SearchHotelsCriteria criteria)
        => (await _repository.SearchHotelsAsync(criteria)).Hotels.Select(h => h.Name).ToList();

    private void Book(Room room, int fromDay, int toDay, BookingStatus status = BookingStatus.Confirmed)
    {
        _db.Bookings.Add(TestData.Booking(room, _guest, CheckIn.AddDays(fromDay), CheckIn.AddDays(toDay), status));
        _db.SaveChanges();
    }

    [Fact]
    public async Task Search_Defaults_ReturnsHotelsWithAnyFittingRoom()
    {
        var (hotels, total) = await _repository.SearchHotelsAsync(Criteria());

        Assert.Equal(3, total);
        Assert.Equal(["Amman Rotana", "Le Marais Boutique", "Downtown Budget"], hotels.Select(h 
            => h.Name));
    }

    [Fact]
    public async Task Search_ReturnsLowestPriceAndCountOfFittingRooms()
    {
        var rotana = (await _repository.SearchHotelsAsync(Criteria())).Hotels.Single(h => h.Name == "Amman Rotana");

        Assert.Equal(220m, rotana.MinPricePerNight);
        Assert.Equal(2, rotana.AvailableRooms);
    }

    [Fact]
    public async Task Search_OneAdult_IncludesSingleRooms()
    {
        var rotana = (await _repository.SearchHotelsAsync(Criteria(r => 
            r.Adults = 1))).Hotels.Single(h => h.Name == "Amman Rotana");

        Assert.Equal(150m, rotana.MinPricePerNight);
        Assert.Equal(3, rotana.AvailableRooms);
    }

    [Fact]
    public async Task Search_IgnoresUnavailableAndDeletedRooms()
    {
        var budget = (await _repository.SearchHotelsAsync(Criteria())).Hotels.Single(h => h.Name == "Downtown Budget");

        Assert.Equal(35m, budget.MinPricePerNight);
        Assert.Equal(1, budget.AvailableRooms);
    }

    [Fact]
    public async Task Search_ProjectsCityThumbnailAndSortedAmenities()
    {
        var rotana = (await _repository.SearchHotelsAsync(Criteria())).Hotels.Single(h => h.Name == "Amman Rotana");

        Assert.Equal("Amman", rotana.CityName);
        Assert.Equal("Jordan", rotana.Country);
        Assert.Equal("rotana-2.png", rotana.ThumbnailUrl);
        Assert.Equal(["Free WiFi", "Spa", "Swimming Pool"], rotana.Amenities);
    }

    // Case-insensitivity comes from the SQL Server collation; the in-memory provider compares case-sensitively.
    [Theory]
    [InlineData("Rotana", new[] { "Amman Rotana" })]
    [InlineData("Amman", new[] { "Amman Rotana", "Downtown Budget" })]
    [InlineData("France", new[] { "Le Marais Boutique" })]
    [InlineData("Tokyo", new string[0])]
    public async Task Search_Query_MatchesHotelCityOrCountry(string q, string[] expected)
    {
        var names = await Names(Criteria(r => r.Q = q));

        Assert.Equal(expected.OrderBy(n => n), names.OrderBy(n => n));
    }

    [Fact]
    public async Task Search_CityId_FiltersByCity()
    {
        var names = await Names(Criteria(r => r.CityId = _paris.CityId));

        Assert.Equal(["Le Marais Boutique"], names);
    }

    [Fact]
    public async Task Search_Stars_FiltersByAnyOfTheValues()
    {
        var names = await Names(Criteria(r => r.Stars = [4, 5]));

        Assert.Equal(["Amman Rotana", "Le Marais Boutique"], names);
    }

    [Fact]
    public async Task Search_Types_FiltersByHotelType()
    {
        var names = await Names(Criteria(r => r.Types = ["budget", "Boutique"]));

        Assert.Equal(["Le Marais Boutique", "Downtown Budget"], names);
    }

    [Fact]
    public async Task Search_Amenities_RequiresAllSelectedAmenities()
    {
        Assert.Equal(["Amman Rotana", "Le Marais Boutique"], await Names(Criteria(r 
            => r.Amenities = [_spa.AmenityId])));
        Assert.Equal(["Amman Rotana"], await Names(Criteria(r 
            => r.Amenities = [_spa.AmenityId, _pool.AmenityId])));
    }

    [Fact]
    public async Task Search_PriceRange_OnlyCountsRoomsInsideTheRange()
    {
        var hotels = (await _repository
            .SearchHotelsAsync(Criteria(r => { r.MinPrice = 200; r.MaxPrice = 300; }))).Hotels;

        var rotana = Assert.Single(hotels);
        Assert.Equal(220m, rotana.MinPricePerNight);
        Assert.Equal(1, rotana.AvailableRooms);
    }

    [Fact]
    public async Task Search_Capacity_ExcludesHotelsWithoutBigEnoughRooms()
    {
        var names = await Names(Criteria(r => { r.Adults = 4; r.Children = 2; }));

        Assert.Equal(["Amman Rotana"], names);
    }

    [Fact]
    public async Task Search_MultipleRooms_SplitsGuestsAndNeedsEnoughRooms()
    {
        Assert.Equal(["Amman Rotana", "Le Marais Boutique"], await Names(Criteria(r => 
            { r.Adults = 4; r.Rooms = 2; })));
        Assert.Empty(await Names(Criteria(r => { r.Adults = 6; r.Rooms = 3; })));
    }

    [Theory]
    [InlineData(BookingStatus.Confirmed)]
    [InlineData(BookingStatus.Pending)]
    public async Task Search_OverlappingActiveBooking_MakesRoomUnavailable(BookingStatus status)
    {
        Book(_rotanaSingle, 1, 2, status);

        var rotana = (await _repository.SearchHotelsAsync(Criteria(r => 
            r.Adults = 1))).Hotels.Single(h => h.Name == "Amman Rotana");

        Assert.Equal(220m, rotana.MinPricePerNight);
        Assert.Equal(2, rotana.AvailableRooms);
    }

    [Theory]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Completed)]
    public async Task Search_InactiveBooking_DoesNotBlockRoom(BookingStatus status)
    {
        Book(_rotanaSingle, 0, 3, status);

        var rotana = (await _repository.SearchHotelsAsync(Criteria(r => 
            r.Adults = 1))).Hotels.Single(h => h.Name == "Amman Rotana");

        Assert.Equal(150m, rotana.MinPricePerNight);
    }

    [Theory]
    [InlineData(-3, 0)]
    [InlineData(3, 5)]
    public async Task Search_BookingTouchingStayBoundary_DoesNotBlockRoom(int fromDay, int toDay)
    {
        Book(_rotanaSingle, fromDay, toDay);

        var rotana = (await _repository.SearchHotelsAsync(Criteria(r => 
            r.Adults = 1))).Hotels.Single(h => h.Name == "Amman Rotana");

        Assert.Equal(3, rotana.AvailableRooms);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(2, 4)]
    [InlineData(-2, 5)]
    [InlineData(1, 2)]
    public async Task Search_BookingOverlappingAnyNight_BlocksRoom(int fromDay, int toDay)
    {
        Book(_rotanaSingle, fromDay, toDay);

        var rotana = (await _repository.SearchHotelsAsync(Criteria(r => 
            r.Adults = 1))).Hotels.Single(h => h.Name == "Amman Rotana");

        Assert.Equal(2, rotana.AvailableRooms);
    }

    [Fact]
    public async Task Search_EveryRoomBooked_RemovesHotel()
    {
        foreach (var room in _db.Rooms.Where(r => r.HotelId == _boutique.HotelId).ToList())
            Book(room, 0, 3);

        Assert.DoesNotContain("Le Marais Boutique", await Names(Criteria()));
    }

    [Fact]
    public async Task Search_DeletedHotel_IsExcluded()
    {
        _budget.IsDeleted = true;
        _db.SaveChanges();

        Assert.DoesNotContain("Downtown Budget", await Names(Criteria()));
    }

    [Theory]
    [InlineData("price_asc", new[] { "Downtown Budget", "Le Marais Boutique", "Amman Rotana" })]
    [InlineData("price_desc", new[] { "Amman Rotana", "Le Marais Boutique", "Downtown Budget" })]
    [InlineData("stars_asc", new[] { "Downtown Budget", "Le Marais Boutique", "Amman Rotana" })]
    [InlineData("stars_desc", new[] { "Amman Rotana", "Le Marais Boutique", "Downtown Budget" })]
    [InlineData("name", new[] { "Amman Rotana", "Downtown Budget", "Le Marais Boutique" })]
    public async Task Search_Sort_OrdersResults(string sort, string[] expected)
    {
        Assert.Equal(expected, await Names(Criteria(r => r.Sort = sort)));
    }

    [Fact]
    public async Task Search_Paging_ReturnsRequestedSliceAndFullTotal()
    {
        var (hotels, total) = await _repository.SearchHotelsAsync(
            Criteria(r => { r.Sort = "name"; r.Page = 2; r.PageSize = 2; }));

        Assert.Equal(3, total);
        Assert.Equal(["Le Marais Boutique"], hotels.Select(h => h.Name));
    }

    public void Dispose() => _db.Dispose();
}
