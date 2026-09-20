using Triply.Application.Common.Models;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Application.Features.Hotels.Commands.UpdateHotel;
using Triply.Application.Features.Hotels.Commands.UploadImage;
using Triply.Application.Features.Hotels.Queries.GetHotels;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the hotel operations.</summary>
public interface IHotelService
{
    /// <summary>Creates a new hotel.</summary>
    Task<Result<HotelResponse>> CreateAsync(CreateHotelRequest request, Guid ownerId
        , CancellationToken cancellationToken = default);
    /// <summary>Gets the hotel by its identifier.</summary>
    Task<Result<HotelResponse>> GetByIdAsync(Guid hotelId, CancellationToken cancellationToken = default);

    /// <summary>Gets a paginated list of hotels.</summary>
    Task<Result<PagedResult<HotelSummaryResponse>>> GetPagedAsync(GetHotelsRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    /// <summary>Updates an existing hotel.</summary>
    Task<Result<HotelResponse>> UpdateAsync(Guid hotelId, UpdateHotelRequest request,
        CancellationToken cancellationToken = default);
    /// <summary>Deletes the hotel.</summary>
    Task<Result<bool>> DeleteAsync(Guid hotelId, CancellationToken cancellationToken = default);

    /// <summary>Initiates the upload.</summary>
    Task<Result<HotelImageResponse>> InitiateUploadAsync(
        Guid hotelId, UploadHotelImageRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the images.</summary>
    Task<Result<IReadOnlyList<HotelImageResponse>>> GetImagesAsync(
        Guid hotelId, CancellationToken cancellationToken = default);

    /// <summary>Deletes the image.</summary>
    Task<Result<bool>> DeleteImageAsync(Guid hotelId, Guid imageId, CancellationToken cancellationToken = default);
}