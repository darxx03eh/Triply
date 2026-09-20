using Triply.Application.DTOs.Attractions;
using Triply.Application.Features.Attractions.Commands.CreateAttraction;
using Triply.Application.Features.Attractions.Commands.UpdateAttraction;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the attraction operations.</summary>
public interface IAttractionService
{
    /// <summary>Creates a new attraction.</summary>
    Task<Result<AttractionResponse>> CreateAsync(CreateAttractionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the hotel attractions.</summary>
    Task<Result<IReadOnlyList<AttractionResponse>>> GetHotelAttractionsAsync(Guid hotelId,
        CancellationToken cancellationToken = default);

    /// <summary>Updates an existing attraction.</summary>
    Task<Result<AttractionResponse>> UpdateAsync(Guid attractionId, UpdateAttractionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes the attraction.</summary>
    Task<Result<bool>> DeleteAsync(Guid attractionId, CancellationToken cancellationToken = default);
}