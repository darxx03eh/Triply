using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    public async Task<Result<IReadOnlyList<HotelImageResponse>>> GetImagesAsync(
        Guid hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
            return Result<IReadOnlyList<HotelImageResponse>>.Failure(
                "HOTEL_NOT_FOUND", "The specified hotel does not exist.", ResultErrorType.NotFound);

        var images = await imageRepository.GetByHotelIdAsync(hotelId, cancellationToken);

        return Result<IReadOnlyList<HotelImageResponse>>.Success(
            images.Select(i => i.ToHotelImageResponse()).ToList(),
            success: new ResultSuccess("HOTEL_IMAGES_FOUND", "Hotel images were retrieved successfully."));
    }
}
