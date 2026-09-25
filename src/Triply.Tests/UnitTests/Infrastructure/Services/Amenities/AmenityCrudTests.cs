using Moq;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Application.Features.Amenities.Commands.UpdateAmenity;
using Triply.Domain.Entities;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Amenities;

public class AmenityCrudTests : AmenityServiceTestBase
{
    [Fact]
    public async Task CreateAsync_SavesTrimmedName()
    {
        Amenity? saved = null;
        AmenityRepository.Setup(r => r.AddAsync(It.IsAny<Amenity>(), It.IsAny<CancellationToken>()))
            .Callback<Amenity, CancellationToken>((a, _) => saved = a);

        var result = await Service.CreateAsync(new CreateAmenityRequest { Name = "  Rooftop Bar " });

        Assert.Equal("Rooftop Bar", result.AssertSuccess(ResultSuccessType.Created).Name);
        Assert.Equal("Rooftop Bar", saved!.Name);
        AmenityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsRepositoryOrder()
    {
        AmenityRepository.Setup(r => r.GetAllOrderedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([TestData.Amenity("Air Conditioning"), TestData.Amenity("Spa")]);

        var amenities = (await Service.GetAllAsync()).AssertSuccess();

        Assert.Equal(["Air Conditioning", "Spa"], amenities.Select(a => a.Name));
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsAmenity()
    {
        var amenity = ExistingAmenity();

        Assert.Equal("Spa", (await Service.GetByIdAsync(amenity.AmenityId)).AssertSuccess().Name);
    }

    [Fact]
    public async Task GetByIdAsync_Missing_ReturnsNotFound()
    {
        (await Service.GetByIdAsync(Guid.NewGuid())).AssertFailure("AMENITY_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_Existing_RenamesAndStampsModifiedAt()
    {
        var amenity = ExistingAmenity();

        var result = await Service.UpdateAsync(amenity.AmenityId, new UpdateAmenityRequest { Name = " Sky Lounge " });

        Assert.Equal("Sky Lounge", result.AssertSuccess().Name);
        Assert.NotNull(amenity.ModifiedAt);
        AmenityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Missing_ReturnsNotFound()
    {
        var result = await Service.UpdateAsync(Guid.NewGuid(), new UpdateAmenityRequest { Name = "X" });

        result.AssertFailure("AMENITY_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_Existing_HardDeletes()
    {
        var amenity = ExistingAmenity();

        var result = await Service.DeleteAsync(amenity.AmenityId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        AmenityRepository.Verify(r => r.DeleteAsync(amenity), Times.Once);
        AmenityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Missing_ReturnsNotFound()
    {
        (await Service.DeleteAsync(Guid.NewGuid())).AssertFailure("AMENITY_NOT_FOUND", 
            ResultErrorType.NotFound);
    }
}
