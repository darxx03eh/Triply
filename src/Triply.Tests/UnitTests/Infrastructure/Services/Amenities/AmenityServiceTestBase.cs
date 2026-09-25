using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Services.Amenities;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Amenities;

public abstract class AmenityServiceTestBase
{
    protected readonly Mock<IAmenityRepository> AmenityRepository = new();
    protected readonly Mock<IHotelRepository> HotelRepository = new();
    protected readonly AmenityService Service;

    protected AmenityServiceTestBase() => Service = new AmenityService(AmenityRepository.Object,
        HotelRepository.Object, NullLogger<AmenityService>.Instance);

    protected Amenity ExistingAmenity(string name = "Spa")
    {
        var amenity = TestData.Amenity(name);
        AmenityRepository.Setup(r => r.GetByIdAsync(amenity.AmenityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(amenity);
        return amenity;
    }
}
