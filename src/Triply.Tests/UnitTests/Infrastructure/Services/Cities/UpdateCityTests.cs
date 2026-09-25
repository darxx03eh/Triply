using Microsoft.EntityFrameworkCore;
using Moq;
using Triply.Application.Features.Cities.Commands.UpdateCity;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Cities;

public class UpdateCityTests : CityServiceTestBase
{
    private static UpdateCityRequest Request(Guid id) => new()
    {
        CityId = id,
        Name = "Nablus City",
        Country = "Palestine",
        PostOffice = "P401",
        RowVersion = [9, 9, 9, 9, 9, 9, 9, 9]
    };

    [Fact]
    public async Task UpdateAsync_Existing_UpdatesFieldsAndUsesClientRowVersion()
    {
        var city = TestData.City();
        CityRepository.Setup(r => 
            r.GetByIdAsync(city.CityId, It.IsAny<CancellationToken>())).ReturnsAsync(city);
        var request = Request(city.CityId);

        var result = await Service.UpdateAsync(city.CityId, request);

        var response = result.AssertSuccess();
        Assert.Equal("Nablus City", response.Name);
        Assert.Equal("P401", city.PostOffice);
        Assert.NotNull(city.ModifiedAt);
        CityRepository.Verify(r => r.SetOriginalRowVersion(city, request.RowVersion), Times.Once);
        CityRepository.Verify(r => 
            r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Missing_ReturnsNotFound()
    {
        var result = await Service.UpdateAsync(Guid.NewGuid(), Request(Guid.NewGuid()));

        result.AssertFailure("CITY_NOT_FOUND", ResultErrorType.NotFound);
        CityRepository.Verify(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_SoftDeleted_ReturnsNotFound()
    {
        var city = TestData.City(isDeleted: true);
        CityRepository.Setup(r => r.GetByIdAsync(city.CityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(city);

        var result = await Service.UpdateAsync(city.CityId, Request(city.CityId));

        result.AssertFailure("CITY_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_ConcurrentEdit_ReturnsConflict()
    {
        var city = TestData.City();
        CityRepository.Setup(r => r.GetByIdAsync(city.CityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(city);
        CityRepository.Setup(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateConcurrencyException());

        var result = await Service.UpdateAsync(city.CityId, Request(city.CityId));

        result.AssertFailure("CITY_CONCURRENCY_CONFLICT", ResultErrorType.Conflict);
    }
}
