using Microsoft.Extensions.Logging;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    /// <summary>Deletes the hotel.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
        {
            logger.LogWarning("Delete hotel failed: hotel {HotelId} was not found", hotelId);
            return Result<bool>.Failure(
                "HOTEL_NOT_FOUND", 
                $"The requested hotel with id: {hotelId.ToString()} was not found.", 
                ResultErrorType.NotFound);
        }

        hotel.IsDeleted = true;
        hotel.ModifiedAt = DateTime.UtcNow;
        await hotelRepository.SoftDeleteRoomsAsync(hotelId, cancellationToken);
        await hotelRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Hotel {HotelId} ({HotelName}) and its rooms soft deleted", hotelId, hotel.Name);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent, 
            new ("HOTEL_DELETED", "Hotel deleted successfully."));
    }
}