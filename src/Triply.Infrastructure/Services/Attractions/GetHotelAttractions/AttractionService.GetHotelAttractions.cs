using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Attractions;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Attractions;

public partial class AttractionService
{
    /// <summary>Gets the hotel attractions.</summary>
    public async Task<Result<IReadOnlyList<AttractionResponse>>> GetHotelAttractionsAsync(Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        if (!await hotelRepository.IsHotelIdExistsAsync(hotelId, cancellationToken))
        {
            logger.LogWarning("Get attractions failed: hotel {HotelId} was not found", hotelId);
            return Result<IReadOnlyList<AttractionResponse>>.Failure(
                "HOTEL_NOT_FOUND",
                $"The requested hotel with id: {hotelId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        var attractions = await attractionRepository.GetByHotelIdAsync(hotelId, cancellationToken);

        return Result<IReadOnlyList<AttractionResponse>>.Success(
            attractions.Select(a => a.ToAttractionResponse()).ToList(),
            success: new(
                "ATTRACTIONS_FOUND", "Nearby attractions were retrieved successfully."));
    }
}