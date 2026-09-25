using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using Triply.Application.DTOs.Search;
using Triply.Application.Extensions;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Application.Interfaces.Repositories;
using Triply.Infrastructure.Services.Search;
using Triply.Tests.UnitTests.Common.Assertions;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Search;

public class SearchHotelsTests
{
    private readonly Mock<ISearchRepository> _searchRepository = new();
    private readonly SearchService _service;
    private SearchHotelsCriteria? _criteria;

    public SearchHotelsTests()
    {
        _service = new SearchService(_searchRepository.Object, NullLogger<SearchService>.Instance);
        _searchRepository.Setup(r => 
                r.SearchHotelsAsync(It.IsAny<SearchHotelsCriteria>(), 
                    It.IsAny<CancellationToken>()))
            .Callback<SearchHotelsCriteria, CancellationToken>((c, _) => _criteria = c)
            .ReturnsAsync((new List<HotelSearchItemResponse>
            {
                new() { Name = "A", MinPricePerNight = 100m },
                new() { Name = "B", MinPricePerNight = 59.5m }
            }, 12));
    }

    [Fact]
    public async Task SearchHotelsAsync_CalculatesTotalPriceForNightsAndRooms()
    {
        var checkIn = SearchExtensions.Today().AddDays(2);
        var request = new SearchHotelsRequest { CheckIn = checkIn, CheckOut = checkIn.AddDays(3), Adults = 4, Rooms = 2 };

        var response = (await _service.SearchHotelsAsync(request)).AssertSuccess();

        Assert.Equal(600m, response.Items[0].TotalPrice);
        Assert.Equal(357m, response.Items[1].TotalPrice);
    }

    [Fact]
    public async Task SearchHotelsAsync_EchoesStayDetailsAndPaging()
    {
        var checkIn = SearchExtensions.Today().AddDays(1);
        var request = new SearchHotelsRequest
        {
            CheckIn = checkIn, CheckOut = checkIn.AddDays(2), Adults = 3, Children = 1, Rooms = 1, Page = 2, PageSize = 5
        };

        var response = (await _service.SearchHotelsAsync(request)).AssertSuccess();

        Assert.Equal(checkIn, response.CheckIn);
        Assert.Equal(checkIn.AddDays(2), response.CheckOut);
        Assert.Equal(2, response.Nights);
        Assert.Equal(3, response.Adults);
        Assert.Equal(1, response.Children);
        Assert.Equal(2, response.Page);
        Assert.Equal(5, response.PageSize);
        Assert.Equal(12, response.TotalCount);
        Assert.Equal(3, response.TotalPages);
    }

    [Fact]
    public async Task SearchHotelsAsync_PassesNormalizedCriteriaToRepository()
    {
        await _service.SearchHotelsAsync(new SearchHotelsRequest { Q = "  Amman ", Types = ["luxury"], Sort = "PRICE_ASC" });

        Assert.Equal("Amman", _criteria!.Q);
        Assert.Equal(SearchSorts.PriceAsc, _criteria.Sort);
        Assert.Equal(Triply.Domain.Enums.Hotels.HotelType.Luxury, _criteria.Types.Single());
    }

    [Fact]
    public async Task SearchHotelsAsync_WithResults_ReturnsFoundMessage()
    {
        var result = await _service.SearchHotelsAsync(new SearchHotelsRequest());

        Assert.Equal("SEARCH_RESULTS_FOUND", result.SuccessObject!.Code);
    }

    [Fact]
    public async Task SearchHotelsAsync_NoResults_ReturnsOkWithNoResultsMessage()
    {
        _searchRepository.Setup(r => r.SearchHotelsAsync(It.IsAny<SearchHotelsCriteria>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<HotelSearchItemResponse>(), 0));

        var result = await _service.SearchHotelsAsync(new SearchHotelsRequest());

        Assert.Empty(result.AssertSuccess().Items);
        Assert.Equal("SEARCH_NO_RESULTS", result.SuccessObject!.Code);
    }
}
