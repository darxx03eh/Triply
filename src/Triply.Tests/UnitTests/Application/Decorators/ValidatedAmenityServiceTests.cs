using Moq;
using Triply.Application.Exceptions;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Application.Features.Amenities.Commands.UpdateAmenity;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Application.Interfaces.Services;
using Triply.Application.Services;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Application.Decorators;

public class ValidatedAmenityServiceTests
{
    private readonly Mock<IAmenityService> _inner = new();

    private ValidatedAmenityService Create(bool valid = true) => new(
        _inner.Object,
        valid ? FakeValidators.Passing<CreateAmenityRequest>() : FakeValidators.Failing<CreateAmenityRequest>(),
        valid ? FakeValidators.Passing<UpdateAmenityRequest>() : FakeValidators.Failing<UpdateAmenityRequest>(),
        valid ? FakeValidators.Passing<SetHotelAmenitiesRequest>() : FakeValidators.Failing<SetHotelAmenitiesRequest>());

    [Fact]
    public async Task ValidatedMethods_Valid_CallInner()
    {
        var service = Create();
        var hotelId = Guid.NewGuid();

        await service.CreateAsync(new CreateAmenityRequest());
        await service.UpdateAsync(Guid.NewGuid(), new UpdateAmenityRequest());
        await service.SetHotelAmenitiesAsync(hotelId, new SetHotelAmenitiesRequest());

        _inner.Verify(s => s.CreateAsync(It.IsAny<CreateAmenityRequest>(), 
            It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), 
            It.IsAny<UpdateAmenityRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.SetHotelAmenitiesAsync(hotelId, 
            It.IsAny<SetHotelAmenitiesRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidatedMethods_Invalid_ThrowAndSkipInner()
    {
        var service = Create(false);

        await Assert.ThrowsAsync<UnprocessableEntityException>(() => service.CreateAsync(new CreateAmenityRequest()));
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => service.UpdateAsync(Guid.NewGuid(), 
            new UpdateAmenityRequest()));
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => service.SetHotelAmenitiesAsync(Guid.NewGuid(), 
            new SetHotelAmenitiesRequest()));

        _inner.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PassThroughMethods_CallInner()
    {
        var id = Guid.NewGuid();
        var service = Create(false);

        await service.GetAllAsync();
        await service.GetByIdAsync(id);
        await service.DeleteAsync(id);
        await service.GetHotelAmenitiesAsync(id);

        _inner.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.GetHotelAmenitiesAsync(id, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
