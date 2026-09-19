using Triply.Application.DTOs.Attractions;
using Triply.Application.Features.Attractions.Commands.CreateAttraction;
using Triply.Application.Features.Attractions.Commands.UpdateAttraction;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

public interface IAttractionService
{
    Task<Result<AttractionResponse>> CreateAsync(CreateAttractionRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AttractionResponse>>> GetHotelAttractionsAsync(Guid hotelId,
        CancellationToken cancellationToken = default);

    Task<Result<AttractionResponse>> UpdateAsync(Guid attractionId, UpdateAttractionRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(Guid attractionId, CancellationToken cancellationToken = default);
}