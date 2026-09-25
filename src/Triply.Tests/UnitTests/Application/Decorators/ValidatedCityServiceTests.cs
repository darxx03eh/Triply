using Moq;
using Triply.Application.Exceptions;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Application.Features.Cities.Commands.UpdateCity;
using Triply.Application.Features.Cities.Commands.UploadCityThumbnail;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Application.Interfaces.Services;
using Triply.Application.Services;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Application.Decorators;

public class ValidatedCityServiceTests
{
    private readonly Mock<ICityService> _inner = new();

    private ValidatedCityService Create(bool valid = true) => new(
        _inner.Object,
        valid ? FakeValidators.Passing<CreateCityRequest>() : FakeValidators.Failing<CreateCityRequest>(),
        valid ? FakeValidators.Passing<UpdateCityRequest>() : FakeValidators.Failing<UpdateCityRequest>(),
        valid ? FakeValidators.Passing<GetCitiesRequest>() : FakeValidators.Failing<GetCitiesRequest>(),
        valid ? FakeValidators.Passing<UploadCityThumbnailRequest>() : FakeValidators.Failing<UploadCityThumbnailRequest>());

    [Fact]
    public async Task CreateAsync_Valid_CallsInner()
    {
        var request = new CreateCityRequest();

        await Create().CreateAsync(request);

        _inner.Verify(s => s.CreateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Invalid_ThrowsAndSkipsInner()
    {
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => Create(valid: false)
            .CreateAsync(new CreateCityRequest()));

        _inner.Verify(s => s.CreateAsync(It.IsAny<CreateCityRequest>(), 
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Invalid_ThrowsAndSkipsInner()
    {
        await Assert.ThrowsAsync<UnprocessableEntityException>(
            () => Create(valid: false).UpdateAsync(Guid.NewGuid(), new UpdateCityRequest()));

        _inner.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), 
            It.IsAny<UpdateCityRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_Invalid_ThrowsAndSkipsInner()
    {
        await Assert.ThrowsAsync<UnprocessableEntityException>(
            () => Create(valid: false).GetPagedAsync(new GetCitiesRequest(), isAdmin: false));

        _inner.Verify(s => s.GetPagedAsync(It.IsAny<GetCitiesRequest>(), 
            It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UploadThumbnailAsync_Invalid_ThrowsAndSkipsInner()
    {
        await Assert.ThrowsAsync<UnprocessableEntityException>(
            () => Create(valid: false).UploadThumbnailAsync(Guid.NewGuid(), new UploadCityThumbnailRequest()));

        _inner.Verify(s => s.UploadThumbnailAsync(It.IsAny<Guid>(), 
            It.IsAny<UploadCityThumbnailRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task PassThroughMethods_CallInnerWithSameArguments()
    {
        var id = Guid.NewGuid();
        var service = Create(valid: false);

        await service.GetByIdAsync(id);
        await service.DeleteAsync(id);
        await service.DeleteThumbnailAsync(id);

        _inner.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.DeleteThumbnailAsync(id, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
