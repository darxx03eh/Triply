using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    public async Task<Result<HotelResponse>> GetByIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdWithImagesAsync(hotelId, cancellationToken);
        if (hotel is null)
            return Result<HotelResponse>.Failure(
                "HOTEL_NOT_FOUND", 
                $"The requested hotel with id: {hotelId.ToString()} was not found.", 
                ResultErrorType.NotFound);

        var imageUrls = hotel.Images
            .OrderBy(i => i.DisplayOrder)
            .Select(i => i.Url!)
            .ToList();
        return Result<HotelResponse>.Success(hotel.ToHotelResponse(hotel.City.Name, imageUrls), success: new(
            "HOTEL_FOUND", $"The requested hotel with id: {hotelId.ToString()} was found."));
    }
}