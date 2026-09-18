using Triply.Application.Common.Models;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Application.Features.Hotels.Commands.UpdateHotel;
using Triply.Application.Features.Hotels.Commands.UploadImage;
using Triply.Application.Features.Hotels.Queries.GetHotels;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

public interface IHotelService
{
    Task<Result<HotelResponse>> CreateAsync(CreateHotelRequest request, Guid ownerId
        , CancellationToken cancellationToken = default);
    Task<Result<HotelResponse>> GetByIdAsync(Guid hotelId, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<HotelSummaryResponse>>> GetPagedAsync(GetHotelsRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<Result<HotelResponse>> UpdateAsync(Guid hotelId, UpdateHotelRequest request,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid hotelId, CancellationToken cancellationToken = default);

    Task<Result<HotelImageResponse>> InitiateUploadAsync(
        Guid hotelId, UploadHotelImageRequest request, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<HotelImageResponse>>> GetImagesAsync(
        Guid hotelId, CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteImageAsync(Guid hotelId, Guid imageId, CancellationToken cancellationToken = default);
}