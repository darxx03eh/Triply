using Moq;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Domain.Entities;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Cities;

public class CreateCityTests : CityServiceTestBase
{
    [Fact]
    public async Task CreateAsync_SavesCityAndReturnsCreated()
    {
        City? saved = null;
        CityRepository.Setup(r => 
                r.AddAsync(It.IsAny<City>(), It.IsAny<CancellationToken>()))
            .Callback<City, CancellationToken>((city, _) => saved = city);

        var result = await Service
            .CreateAsync(new CreateCityRequest { Name = "Jericho", Country = "Palestine", PostOffice = "P900" });

        var response = result.AssertSuccess(ResultSuccessType.Created);
        Assert.Equal("CITY_CREATED", result.SuccessObject!.Code);
        Assert.NotNull(saved);
        Assert.Equal("Jericho", saved!.Name);
        Assert.Equal(saved.CityId, response.CityId);
        Assert.Equal(0, response.HotelsCount);
        CityRepository.Verify(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
