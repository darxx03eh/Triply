using Triply.Application.DTOs.Hotels;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for hotel.</summary>
public static class HotelExtensions
{
    /// <summary>Maps the hotel to a hotel response.</summary>
    public static HotelResponse ToHotelResponse(this Hotel hotel, string cityName, IReadOnlyList<string> imageUrls,
        decimal? averageRating = null, int reviewsCount = 0)
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
            RowVersion = hotel.RowVersion,
            HotelType = hotel.HotelType,
            Address = hotel.Address,
            Amenities = hotel.HotelAmenities
                .Where(ha => ha.Amenity is not null)
                .Select(ha => ha.Amenity.Name)
                .OrderBy(name => name)
                .ToList(),
            AverageRating = averageRating,
            ReviewsCount = reviewsCount,
            Latitude =  hotel.Latitude,
            Longitude =  hotel.Longitude,
            OwnerId = hotel.OwnerId,
            StarRating =  hotel.StarRating,
        };

    /// <summary>Maps the hotel to a hotel summary response.</summary>
    public static HotelSummaryResponse ToHotelSummaryResponse(this Hotel hotel, string cityName,
        int roomsCount = 0, string? thumbnailUrl = null)
        => new HotelSummaryResponse()
        {
            HotelId =  hotel.HotelId,
            Name = hotel.Name,
            CityName =  cityName,
            OwnerId = hotel.OwnerId,
            OwnerName = hotel.Owner is null ? null : $"{hotel.Owner.FirstName} {hotel.Owner.LastName}",
            RoomsCount = roomsCount,
            IsDeleted = hotel.IsDeleted,
            Latitude =  hotel.Latitude,
            Longitude =  hotel.Longitude,
            CreatedAt =  hotel.CreatedAt,
            ModifiedAt = hotel.ModifiedAt,
            StarRating = hotel.StarRating,
            HotelType = hotel.HotelType,
            Address = hotel.Address,
            ThumbnailUrl = thumbnailUrl
        };

    /// <summary>Maps the hotel to a hotel image response.</summary>
    public static HotelImageResponse ToHotelImageResponse(this HotelImage image)
        => new HotelImageResponse
        {
            ImageId = image.ImageId,
            HotelId = image.HotelId,
            Url = image.Url,
            DisplayOrder = image.DisplayOrder,
            Status = image.Status
        };

    /// <summary>Maps the Create hotel request to a hotel.</summary>
    public static Hotel ToHotel(this CreateHotelRequest request, Guid ownerId)
        => new Hotel()
        {
            Name = request.Name.Trim(),
            CityId = request.CityId,
            OwnerId = ownerId,
            StarRating = request.StarRating,
            HotelType = request.HotelType,
            Address = request.Address.Trim(),
            Description = request.Description.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };
}