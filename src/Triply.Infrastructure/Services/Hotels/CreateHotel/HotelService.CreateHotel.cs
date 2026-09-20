using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

/// <summary>Implements the hotel operations.</summary>
public partial class HotelService
{
    /// <summary>Creates a new hotel.</summary>
    public async Task<Result<HotelResponse>> CreateAsync(CreateHotelRequest request,
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        var city = await cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
        {
            logger.LogWarning("Create hotel {HotelName} failed: city {CityId} does not exist", 
                request.Name, request.CityId);
            return Result<HotelResponse>.Failure(
                "CITY_NOT_FOUND", $"The specified city with id: {request.CityId.ToString()} does not exist.",
                type: ResultErrorType.NotFound);
        }

        var hotel = request.ToHotel(ownerId);

        await hotelRepository.AddAsync(hotel, cancellationToken);
        await hotelRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Hotel {HotelId} ({HotelName}, {StarRating} stars) created in {CityName} by {OwnerId}",
            hotel.HotelId, hotel.Name, hotel.StarRating, city.Name, ownerId);

        return Result<HotelResponse>.Success(
            hotel.ToHotelResponse(city.Name, []), ResultSuccessType.Created,
            new("HOTEL_CREATED", "Hotel created successfully."));
    }
}