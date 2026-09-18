using Microsoft.EntityFrameworkCore;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Commands.UpdateHotel;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    public async Task<Result<HotelResponse>> UpdateAsync(Guid hotelId, UpdateHotelRequest request,
        CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
            return Result<HotelResponse>.Failure("HOTEL_NOT_FOUND", 
                $"The requested hotel with id: {hotelId.ToString()} was not found.",
                ResultErrorType.NotFound);

        City city;
        if (request.CityId != hotel.CityId)
        {
            var newCity = await cityRepository.GetByIdAsync(request.CityId, cancellationToken);
            if (newCity is null)
                return Result<HotelResponse>.Failure(
                    "CITY_NOT_FOUND", 
                    "The specified city does not exist.", 
                    type: ResultErrorType.BusinessRule);
            city = newCity;
        }
        else city = hotel.City;
        
        hotel.Name = request.Name;
        hotel.CityId = request.CityId;
        hotel.OwnerId = request.OwnerId;
        hotel.StarRating = request.StarRating;
        hotel.Description = request.Description;
        hotel.Latitude = request.Latitude;
        hotel.Longitude = request.Longitude;
        hotel.ModifiedAt = DateTime.UtcNow;

        hotelRepository.SetOriginalRowVersion(hotel, request.RowVersion);
        hotelRepository.UpdateAsync(hotel);

        try
        {
            await hotelRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<HotelResponse>.Failure(
                "HOTEL_CONCURRENCY_CONFLICT",
                "This hotel was modified by someone else. Refresh and try again.",
                ResultErrorType.Conflict);
        }

        return Result<HotelResponse>.Success(
            hotel.ToHotelResponse(city.Name, []), ResultSuccessType.Ok,
            new ResultSuccess("HOTEL_UPDATED", "Hotel updated successfully."));
    }
}