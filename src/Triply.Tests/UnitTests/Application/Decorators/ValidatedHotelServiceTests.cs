using Moq;
using Triply.Application.Exceptions;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Application.Features.Hotels.Commands.UpdateHotel;
using Triply.Application.Features.Hotels.Commands.UploadImage;
using Triply.Application.Features.Hotels.Queries.GetHotels;
using Triply.Application.Interfaces.Services;
using Triply.Application.Services;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Application.Decorators;

public class ValidatedHotelServiceTests
{
    private readonly Mock<IHotelService> _inner = new();

    private ValidatedHotelService Create(bool valid = true) => new(
        _inner.Object,
        valid ? FakeValidators.Passing<CreateHotelRequest>() : FakeValidators.Failing<CreateHotelRequest>(),
        valid ? FakeValidators.Passing<UpdateHotelRequest>() : FakeValidators.Failing<UpdateHotelRequest>(),
        valid ? FakeValidators.Passing<GetHotelsRequest>() : FakeValidators.Failing<GetHotelsRequest>(),
        valid ? FakeValidators.Passing<UploadHotelImageRequest>() : FakeValidators.Failing<UploadHotelImageRequest>());

    [Fact]
    public async Task CreateAsync_Valid_CallsInnerWithOwner()
    {
        var request = new CreateHotelRequest();
        var ownerId = Guid.NewGuid();

        await Create().CreateAsync(request, ownerId);

        _inner.Verify(s => s.CreateAsync(request, ownerId, 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Invalid_ThrowsAndSkipsInner()
    {
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => Create(false).CreateAsync(
            new CreateHotelRequest(), Guid.NewGuid()));

        _inner.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_Invalid_ThrowsAndSkipsInner()
    {
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => Create(false)
            .UpdateAsync(Guid.NewGuid(), new UpdateHotelRequest()));

        _inner.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetPagedAsync_Invalid_ThrowsAndSkipsInner()
    {
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => Create(false)
            .GetPagedAsync(new GetHotelsRequest(), true));

        _inner.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InitiateUploadAsync_Invalid_ThrowsAndSkipsInner()
    {
        await Assert.ThrowsAsync<UnprocessableEntityException>(
            () => Create(false).InitiateUploadAsync(Guid.NewGuid(), new UploadHotelImageRequest()));

        _inner.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PassThroughMethods_CallInner()
    {
        var hotelId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var service = Create(false);

        await service.GetByIdAsync(hotelId);
        await service.DeleteAsync(hotelId);
        await service.GetImagesAsync(hotelId);
        await service.DeleteImageAsync(hotelId, imageId);

        _inner.Verify(s => s.GetByIdAsync(hotelId, It.IsAny<CancellationToken>()), 
            Times.Once);
        _inner.Verify(s => s.DeleteAsync(hotelId, It.IsAny<CancellationToken>()), 
            Times.Once);
        _inner.Verify(s => s.GetImagesAsync(hotelId, It.IsAny<CancellationToken>()), 
            Times.Once);
        _inner.Verify(s => s.DeleteImageAsync(hotelId, imageId, 
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
