using Triply.Application.DTOs.Attractions;
using Triply.Application.Features.Attractions.Commands.CreateAttraction;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for attraction.</summary>
public static class AttractionExtensions
{
    /// <summary>Maps the attraction to a attraction response.</summary>
    public static AttractionResponse ToAttractionResponse(this Attraction attraction)
        => new AttractionResponse()
        {
            AttractionId = attraction.AttractionId,
            HotelId = attraction.HotelId,
            Name = attraction.Name,
            Category = attraction.Category,
            DistanceKm = attraction.DistanceKm
        };

    /// <summary>Maps the attraction to a attraction.</summary>
    public static Attraction ToAttraction(this CreateAttractionRequest request)
        => new Attraction()
        {
            HotelId = request.HotelId,
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            DistanceKm = request.DistanceKm
        };
}