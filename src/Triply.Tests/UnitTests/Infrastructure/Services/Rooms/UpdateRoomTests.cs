using Microsoft.EntityFrameworkCore;
using Moq;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Domain.Enums.Rooms;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Rooms;

public class UpdateRoomTests : RoomServiceTestBase
{
    private static UpdateRoomRequest Request() => new()
    {
        Number = " 202 ",
        RoomType = RoomType.Single,
        AdultCapacity = 1,
        ChildCapacity = 0,
        PricePerNight = 60m,
        IsAvailable = false,
        Description = "Updated",
        RowVersion = [3, 3, 3, 3, 3, 3, 3, 3]
    };

    [Fact]
    public async Task UpdateAsync_Existing_UpdatesAllFields()
    {
        var room = ExistingRoom();
        var request = Request();

        var response = (await Service.UpdateAsync(room.RoomId, request)).AssertSuccess();

        Assert.Equal("202", room.Number);
        Assert.Equal(RoomType.Single, room.RoomType);
        Assert.Equal(60m, room.PricePerNight);
        Assert.False(room.IsAvailable);
        Assert.NotNull(room.ModifiedAt);
        Assert.Equal("Red Sea Resort", response.HotelName);
        RoomRepository.Verify(r => r.SetOriginalRowVersion(room, request.RowVersion), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Missing_ReturnsNotFound()
    {
        var result = await Service.UpdateAsync(Guid.NewGuid(), Request());

        result.AssertFailure("ROOM_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_ConcurrentEdit_ReturnsConflict()
    {
        var room = ExistingRoom();
        RoomRepository.Setup(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateConcurrencyException());

        var result = await Service.UpdateAsync(room.RoomId, Request());

        result.AssertFailure("ROOM_CONCURRENCY_CONFLICT", ResultErrorType.Conflict);
    }
}
