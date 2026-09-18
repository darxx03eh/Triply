using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    public async Task<Result<HotelResponse>> CreateAsync(CreateHotelRequest request,
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        var city = await cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
            return Result<HotelResponse>.Failure(
                "CITY_NOT_FOUND", $"The specified city with id: {request.CityId.ToString()} does not exist.",
                type: ResultErrorType.NotFound);

        var hotel = new Hotel
        {
            Name = request.Name,
            CityId = request.CityId,
            OwnerId = ownerId,
            StarRating = request.StarRating,
            HotelType = request.HotelType,
            Address = request.Address.Trim(),
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        await hotelRepository.AddAsync(hotel, cancellationToken);
        await hotelRepository.SaveChangesAsync(cancellationToken);

        return Result<HotelResponse>.Success(
            hotel.ToHotelResponse(city.Name, []), ResultSuccessType.Created,
            new("HOTEL_CREATED", "Hotel created successfully."));
    }
}