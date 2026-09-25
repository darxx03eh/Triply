using Moq;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Hotels;

public class DeleteHotelTests : HotelServiceTestBase
{
    [Fact]
    public async Task DeleteAsync_Existing_SoftDeletesHotelAndItsRoomsInOneSave()
    {
        var hotel = ExistingHotel();
        var sequence = new MockSequence();
        HotelRepository.InSequence(sequence).Setup(r => r.SoftDeleteRoomsAsync(hotel.HotelId, 
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        HotelRepository.InSequence(sequence).Setup(r => 
            r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await Service.DeleteAsync(hotel.HotelId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        Assert.True(hotel.IsDeleted);
        Assert.NotNull(hotel.ModifiedAt);
        HotelRepository.Verify(r => r.SoftDeleteRoomsAsync(hotel.HotelId, It.IsAny<CancellationToken>()), Times.Once);
        HotelRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Missing_ReturnsNotFound()
    {
        var result = await Service.DeleteAsync(Guid.NewGuid());

        result.AssertFailure("HOTEL_NOT_FOUND", ResultErrorType.NotFound);
        HotelRepository.Verify(r => r.SoftDeleteRoomsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
