using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Amenities;
using Triply.Application.Extensions;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Amenities;

public partial class AmenityService
{
    /// <summary>Gets all amenities.</summary>
    public async Task<Result<IReadOnlyList<AmenityResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var amenities = await amenityRepository.GetAllOrderedAsync(cancellationToken);
        logger.LogDebug("Returned {Count} amenities", amenities.Count);

        return Result<IReadOnlyList<AmenityResponse>>.Success(
            amenities.Select(a => a.ToAmenityResponse()).ToList(),
            success: new("AMENITIES_FOUND", "Amenities were retrieved successfully."));
    }
}
