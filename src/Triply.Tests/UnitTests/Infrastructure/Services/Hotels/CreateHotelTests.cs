using Moq;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Hotels;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Hotels;

public class CreateHotelTests : HotelServiceTestBase
{
    private CreateHotelRequest Request() => new()
    {
        Name = "Red Sea Resort",
        CityId = City.CityId,
        StarRating = 5,
        HotelType = HotelType.Luxury,
        Address = "  South Beach Road  ",
        Description = "Beach",
        Latitude = 29.5m,
        Longitude = 35m
    };

    [Fact]
    public async Task CreateAsync_ValidCity_SavesHotelWithOwnerAndReturnsCreated()
    {
        var ownerId = Guid.NewGuid();
        Hotel? saved = null;
        CityRepository.Setup(r => r.GetByIdAsync(City.CityId, It.IsAny<CancellationToken>())).ReturnsAsync(City);
        HotelRepository.Setup(r => r.AddAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()))
            .Callback<Hotel, CancellationToken>((h, _) => saved = h);

        var result = await Service.CreateAsync(Request(), ownerId);

        var response = result.AssertSuccess(ResultSuccessType.Created);
        Assert.Equal(ownerId, saved!.OwnerId);
        Assert.Equal("South Beach Road", saved.Address);
        Assert.Equal(HotelType.Luxury, saved.HotelType);
        Assert.Equal("Nablus", response.CityName);
        Assert.Empty(response.ImageUrls);
        HotelRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_UnknownCity_ReturnsNotFoundAndSavesNothing()
    {
        var result = await Service.CreateAsync(Request(), Guid.NewGuid());

        result.AssertFailure("CITY_NOT_FOUND", ResultErrorType.NotFound);
        HotelRepository.Verify(r => r.AddAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
