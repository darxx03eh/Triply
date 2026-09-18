using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Amenities;

public partial class AmenityService
{
    public async Task<Result<bool>> DeleteAsync(Guid amenityId, CancellationToken cancellationToken = default)
    {
        var amenity = await amenityRepository.GetByIdAsync(amenityId, cancellationToken);
        if (amenity is null)
            return Result<bool>.Failure(
                "AMENITY_NOT_FOUND",
                $"The requested amenity with id: {amenityId.ToString()} was not found.",
                ResultErrorType.NotFound);

        await amenityRepository.DeleteAsync(amenity);
        await amenityRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ("AMENITY_DELETED", "Amenity deleted successfully."));
    }
}
