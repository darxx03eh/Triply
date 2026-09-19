using FluentValidation;
using Triply.Application.DTOs.Attractions;
using Triply.Application.Extensions;
using Triply.Application.Features.Attractions.Commands.CreateAttraction;
using Triply.Application.Features.Attractions.Commands.UpdateAttraction;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

public class ValidatedAttractionService(
    IAttractionService inner,
    IEnumerable<IValidator<CreateAttractionRequest>> createValidators,
    IEnumerable<IValidator<UpdateAttractionRequest>> updateValidators) : IAttractionService
{
    public async Task<Result<AttractionResponse>> CreateAsync(CreateAttractionRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CreateAsync(request, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<AttractionResponse>>> GetHotelAttractionsAsync(Guid hotelId,
        CancellationToken cancellationToken = default)
        => await inner.GetHotelAttractionsAsync(hotelId, cancellationToken);

    public async Task<Result<AttractionResponse>> UpdateAsync(Guid attractionId, UpdateAttractionRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(attractionId, request, cancellationToken);
    }

    public async Task<Result<bool>> DeleteAsync(Guid attractionId, CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(attractionId, cancellationToken);
}
