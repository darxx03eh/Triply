using FluentValidation;
using Triply.Application.DTOs.Attractions;
using Triply.Application.Extensions;
using Triply.Application.Features.Attractions.Commands.CreateAttraction;
using Triply.Application.Features.Attractions.Commands.UpdateAttraction;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

/// <summary>Gets or sets the validated attraction service.</summary>
/// <summary>Validates the requests before delegating to the attraction service.</summary>
public class ValidatedAttractionService(
    IAttractionService inner,
    IEnumerable<IValidator<CreateAttractionRequest>> createValidators,
    IEnumerable<IValidator<UpdateAttractionRequest>> updateValidators) : IAttractionService
{
    /// <summary>Creates a new attraction.</summary>
    public async Task<Result<AttractionResponse>> CreateAsync(CreateAttractionRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CreateAsync(request, cancellationToken);
    }

    /// <summary>Gets the hotel attractions.</summary>
    public async Task<Result<IReadOnlyList<AttractionResponse>>> GetHotelAttractionsAsync(Guid hotelId,
        CancellationToken cancellationToken = default)
        => await inner.GetHotelAttractionsAsync(hotelId, cancellationToken);

    /// <summary>Updates an existing attraction.</summary>
    public async Task<Result<AttractionResponse>> UpdateAsync(Guid attractionId, UpdateAttractionRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(attractionId, request, cancellationToken);
    }

    /// <summary>Deletes the attraction.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid attractionId, CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(attractionId, cancellationToken);
}
