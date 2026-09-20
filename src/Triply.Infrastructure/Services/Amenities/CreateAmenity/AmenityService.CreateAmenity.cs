using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Amenities;
using Triply.Application.Extensions;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Amenities;

public partial class AmenityService
{
    /// <summary>Creates a new amenity.</summary>
    public async Task<Result<AmenityResponse>> CreateAsync(CreateAmenityRequest request,
        CancellationToken cancellationToken = default)
    {
        var amenity = request.ToAmenity();

        await amenityRepository.AddAsync(amenity, cancellationToken);
        await amenityRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Amenity {AmenityId} ({AmenityName}) created", amenity.AmenityId, amenity.Name);

        return Result<AmenityResponse>.Success(
            amenity.ToAmenityResponse(), ResultSuccessType.Created,
            new ResultSuccess("AMENITY_CREATED", "Amenity created successfully."));
    }
}
