using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Attractions;

public partial class AttractionService
{
    public async Task<Result<bool>> DeleteAsync(Guid attractionId, CancellationToken cancellationToken = default)
    {
        var attraction = await attractionRepository.GetByIdAsync(attractionId, cancellationToken);
        if (attraction is null)
            return Result<bool>.Failure(
                "ATTRACTION_NOT_FOUND",
                $"The requested attraction with id: {attractionId.ToString()} was not found.",
                ResultErrorType.NotFound);

        await attractionRepository.DeleteAsync(attraction);
        await attractionRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ("ATTRACTION_DELETED", "Attraction deleted successfully."));
    }
}