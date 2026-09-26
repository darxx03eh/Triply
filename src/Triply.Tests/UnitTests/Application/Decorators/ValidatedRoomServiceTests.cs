using Moq;
using Triply.Application.Exceptions;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Application.Features.Rooms.Commands.UploadImage;
using Triply.Application.Features.Rooms.Queries.GetRooms;
using Triply.Application.Interfaces.Services;
using Triply.Application.Services;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Application.Decorators;

public class ValidatedRoomServiceTests
{
    private readonly Mock<IRoomService> _inner = new();

    private ValidatedRoomService Create(bool valid = true) => new(
        _inner.Object,
        valid ? FakeValidators.Passing<CreateRoomRequest>() : FakeValidators.Failing<CreateRoomRequest>(),
        valid ? FakeValidators.Passing<UpdateRoomRequest>() : FakeValidators.Failing<UpdateRoomRequest>(),
        valid ? FakeValidators.Passing<GetRoomsRequest>() : FakeValidators.Failing<GetRoomsRequest>(),
        valid ? FakeValidators.Passing<UploadRoomImageRequest>() : FakeValidators.Failing<UploadRoomImageRequest>());

    [Fact]
    public async Task ValidatedMethods_Valid_CallInner()
    {
        var service = Create();
        var hotelId = Guid.NewGuid();

        await service.CreateAsync(new CreateRoomRequest());
        await service.UpdateAsync(Guid.NewGuid(), new UpdateRoomRequest());
        await service.GetPagedAsync(new GetRoomsRequest(), true);
        await service.GetHotelRoomsAsync(hotelId, new GetRoomsRequest());

        _inner.Verify(s => s.CreateAsync(It.IsAny<CreateRoomRequest>(), 
            It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateRoomRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.GetPagedAsync(It.IsAny<GetRoomsRequest>(), true, 
            It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.GetHotelRoomsAsync(hotelId, It.IsAny<GetRoomsRequest>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidatedMethods_Invalid_ThrowAndSkipInner()
    {
        var service = Create(false);

        await Assert.ThrowsAsync<UnprocessableEntityException>(() => service.CreateAsync(new CreateRoomRequest()));
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => service.UpdateAsync(Guid.NewGuid(), 
            new UpdateRoomRequest()));
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => service.GetPagedAsync(new GetRoomsRequest(), 
            false));
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => service.GetHotelRoomsAsync(Guid.NewGuid(), 
            new GetRoomsRequest()));

        _inner.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PassThroughMethods_CallInner()
    {
        var id = Guid.NewGuid();

        await Create(false).GetByIdAsync(id);
        await Create(false).DeleteAsync(id);

        _inner.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
