using Triply.Application.DTOs.Attractions;
using Triply.Application.Extensions;
using Triply.Application.Features.Attractions.Commands.UpdateAttraction;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Attractions;

public partial class AttractionService
{
    public async Task<Result<AttractionResponse>> UpdateAsync(Guid attractionId, UpdateAttractionRequest request,
        CancellationToken cancellationToken = default)
    {
        var attraction = await attractionRepository.GetByIdAsync(attractionId, cancellationToken);
        if (attraction is null)
            return Result<AttractionResponse>.Failure(
                "ATTRACTION_NOT_FOUND",
                $"The requested attraction with id: {attractionId.ToString()} was not found.",
                ResultErrorType.NotFound);

        attraction.Name = request.Name.Trim();
        attraction.Category = request.Category.Trim();
        attraction.DistanceKm = request.DistanceKm;
        attraction.ModifiedAt = DateTime.UtcNow;
        await attractionRepository.SaveChangesAsync(cancellationToken);

        return Result<AttractionResponse>.Success(
            attraction.ToAttractionResponse(), ResultSuccessType.Ok,
            new ResultSuccess("ATTRACTION_UPDATED", "Attraction updated successfully."));
    }
}