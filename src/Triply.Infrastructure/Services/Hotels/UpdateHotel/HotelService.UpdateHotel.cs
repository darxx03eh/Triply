using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Commands.UpdateHotel;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    /// <summary>Updates an existing hotel.</summary>
    public async Task<Result<HotelResponse>> UpdateAsync(Guid hotelId, UpdateHotelRequest request,
        CancellationToken cancellationToken = default)
    {
        // Load the city and the gallery too: the response needs both, and a plain FindAsync
        // leaves hotel.City null (which used to throw when the city was not changed).
        var hotel = await hotelRepository.GetByIdWithImagesAsync(hotelId, cancellationToken);
        if (hotel is null)
        {
            logger.LogWarning("Update hotel failed: hotel {HotelId} was not found", hotelId);
            return Result<HotelResponse>.Failure("HOTEL_NOT_FOUND",
                $"The requested hotel with id: {hotelId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        if (request.CityId != hotel.CityId)
        {
            var newCity = await cityRepository.GetByIdAsync(request.CityId, cancellationToken);
            if (newCity is null)
            {
                logger.LogWarning("Update hotel {HotelId} failed: city {CityId} does not exist", hotelId,
                    request.CityId);
                return Result<HotelResponse>.Failure(
                    "CITY_NOT_FOUND",
                    $"The specified city with id: {request.CityId.ToString()} does not exist.",
                    ResultErrorType.NotFound);
            }
            hotel.City = newCity;
        }

        hotel.Name = request.Name;
        hotel.CityId = request.CityId;
        hotel.OwnerId = request.OwnerId;
        hotel.StarRating = request.StarRating;
        hotel.HotelType = request.HotelType;
        hotel.Address = request.Address.Trim();
        hotel.Description = request.Description;
        hotel.Latitude = request.Latitude;
        hotel.Longitude = request.Longitude;
        hotel.ModifiedAt = DateTime.UtcNow;

        // The hotel is already tracked, so SaveChanges only updates the changed columns.
        // Calling DbSet.Update here would also mark the loaded City and images as modified.
        hotelRepository.SetOriginalRowVersion(hotel, request.RowVersion);

        try
        {
            await hotelRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning("Update hotel {HotelId} rejected: it was modified by someone else (concurrency conflict)", 
                hotelId);
            return Result<HotelResponse>.Failure(
                "HOTEL_CONCURRENCY_CONFLICT",
                "This hotel was modified by someone else. Refresh and try again.",
                ResultErrorType.Conflict);
        }

        logger.LogInformation("Hotel {HotelId} ({HotelName}) updated", hotelId, hotel.Name);
        var imageUrls = hotel.Images
            .OrderBy(i => i.DisplayOrder)
            .Select(i => i.Url!)
            .ToList();

        return Result<HotelResponse>.Success(
            hotel.ToHotelResponse(hotel.City.Name, imageUrls), ResultSuccessType.Ok,
            new ResultSuccess("HOTEL_UPDATED", "Hotel updated successfully."));
    }
}
