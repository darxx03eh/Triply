using Moq;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Rooms;

public class DeleteRoomTests : RoomServiceTestBase
{
    [Fact]
    public async Task DeleteAsync_Existing_SoftDeletes()
    {
        var room = ExistingRoom();

        var result = await Service.DeleteAsync(room.RoomId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        Assert.True(room.IsDeleted);
        Assert.NotNull(room.ModifiedAt);
        RoomRepository.Verify(r => r.DeleteAsync(It.IsAny<Triply.Domain.Entities.Room>()), Times.Never);
        RoomRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Missing_ReturnsNotFound()
    {
        var result = await Service.DeleteAsync(Guid.NewGuid());

        result.AssertFailure("ROOM_NOT_FOUND", ResultErrorType.NotFound);
    }
}
