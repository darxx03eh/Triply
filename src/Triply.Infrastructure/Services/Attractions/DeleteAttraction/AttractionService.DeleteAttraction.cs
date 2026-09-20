using Microsoft.Extensions.Logging;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Attractions;

public partial class AttractionService
{
    /// <summary>Deletes the attraction.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid attractionId, CancellationToken cancellationToken = default)
    {
        var attraction = await attractionRepository.GetByIdAsync(attractionId, cancellationToken);
        if (attraction is null)
        {
            logger.LogWarning("Delete attraction failed: attraction {AttractionId} was not found", attractionId);
            return Result<bool>.Failure(
                "ATTRACTION_NOT_FOUND",
                $"The requested attraction with id: {attractionId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        await attractionRepository.DeleteAsync(attraction);
        await attractionRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Attraction {AttractionId} ({AttractionName}) removed from hotel {HotelId}",
            attractionId, attraction.Name, attraction.HotelId);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ("ATTRACTION_DELETED", "Attraction deleted successfully."));
    }
}