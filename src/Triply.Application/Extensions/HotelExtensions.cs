using Triply.Application.DTOs.Hotels;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

public static class HotelExtensions
{
    public static HotelResponse ToHotelResponse(this Hotel hotel, string cityName, IReadOnlyList<string> imageUrls)
        => new HotelResponse()
        {
            HotelId = hotel.HotelId,
            Name = hotel.Name,
            Description = hotel.Description,
            CityId =  hotel.CityId,
            CityName = cityName,
            ImageUrls = imageUrls,
            CreatedAt =  hotel.CreatedAt,
            ModifiedAt = hotel.ModifiedAt,
            Latitude =  hotel.Latitude,
            Longitude =  hotel.Longitude,
            OwnerId = hotel.OwnerId,
            StarRating =  hotel.StarRating,
        };

    public static HotelSummaryResponse ToHotelSummaryResponse(this Hotel hotel, string cityName)
        => new HotelSummaryResponse()
        {
            HotelId =  hotel.HotelId,
            Name = hotel.Name,
            CityName =  cityName,
            Latitude =  hotel.Latitude,
            Longitude =  hotel.Longitude,
            CreatedAt =  hotel.CreatedAt,
            StarRating = hotel.StarRating
        };
}