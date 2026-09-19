using Triply.Application.DTOs.Attractions;
using Triply.Application.Extensions;
using Triply.Application.Features.Attractions.Commands.CreateAttraction;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Attractions;

public partial class AttractionService
{
    public async Task<Result<AttractionResponse>> CreateAsync(CreateAttractionRequest request,
        CancellationToken cancellationToken = default)
    {
        var attraction = request.ToAttraction();

        await attractionRepository.AddAsync(attraction, cancellationToken);
        await attractionRepository.SaveChangesAsync(cancellationToken);

        return Result<AttractionResponse>.Success(
            attraction.ToAttractionResponse(), ResultSuccessType.Created,
            new ResultSuccess("ATTRACTION_CREATED", "Attraction created successfully."));
    }
}