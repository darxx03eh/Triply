using Triply.Application.DTOs.Amenities;
using Triply.Application.Extensions;
using Triply.Application.Features.Amenities.Commands.UpdateAmenity;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Amenities;

public partial class AmenityService
{
    public async Task<Result<AmenityResponse>> UpdateAsync(Guid amenityId, UpdateAmenityRequest request,
        CancellationToken cancellationToken = default)
    {
        var amenity = await amenityRepository.GetByIdAsync(amenityId, cancellationToken);
        if (amenity is null)
            return Result<AmenityResponse>.Failure(
                "AMENITY_NOT_FOUND",
                $"The requested amenity with id: {amenityId.ToString()} was not found.",
                ResultErrorType.NotFound);

        amenity.Name = request.Name.Trim();
        amenity.ModifiedAt = DateTime.UtcNow;
        await amenityRepository.SaveChangesAsync(cancellationToken);

        return Result<AmenityResponse>.Success(
            amenity.ToAmenityResponse(), ResultSuccessType.Ok,
            new ResultSuccess("AMENITY_UPDATED", "Amenity updated successfully."));
    }
}
