using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    public async Task<Result<bool>> DeleteAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
            return Result<bool>.Failure(
                "HOTEL_NOT_FOUND", 
                $"The requested hotel with id: {hotelId.ToString()} was not found.", 
                ResultErrorType.NotFound);

        hotel.IsDeleted = true;
        hotel.ModifiedAt = DateTime.UtcNow;
        await hotelRepository.SoftDeleteRoomsAsync(hotelId, cancellationToken);
        await hotelRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent, 
            new ("HOTEL_DELETED", "Hotel deleted successfully."));
    }
}