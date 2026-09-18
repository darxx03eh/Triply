using Triply.Application.DTOs.Amenities;
using Triply.Application.Extensions;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Amenities;

public partial class AmenityService
{
    public async Task<Result<AmenityResponse>> CreateAsync(CreateAmenityRequest request,
        CancellationToken cancellationToken = default)
    {
        var amenity = new Amenity { Name = request.Name.Trim() };

        await amenityRepository.AddAsync(amenity, cancellationToken);
        await amenityRepository.SaveChangesAsync(cancellationToken);

        return Result<AmenityResponse>.Success(
            amenity.ToAmenityResponse(), ResultSuccessType.Created,
            new ResultSuccess("AMENITY_CREATED", "Amenity created successfully."));
    }
}
