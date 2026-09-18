using Triply.Application.DTOs.Amenities;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Amenities;

public partial class AmenityService
{
    public async Task<Result<AmenityResponse>> GetByIdAsync(Guid amenityId,
        CancellationToken cancellationToken = default)
    {
        var amenity = await amenityRepository.GetByIdAsync(amenityId, cancellationToken);
        if (amenity is null)
            return Result<AmenityResponse>.Failure(
                "AMENITY_NOT_FOUND",
                $"The requested amenity with id: {amenityId.ToString()} was not found.",
                ResultErrorType.NotFound);

        return Result<AmenityResponse>.Success(amenity.ToAmenityResponse(), success: new(
            "AMENITY_FOUND", $"The requested amenity with id: {amenityId.ToString()} was found."));
    }
}
