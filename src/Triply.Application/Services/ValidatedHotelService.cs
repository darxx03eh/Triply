using FluentValidation;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Application.Features.Hotels.Commands.UpdateHotel;
using Triply.Application.Features.Hotels.Commands.UploadImage;
using Triply.Application.Features.Hotels.Queries.GetHotels;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

public class ValidatedHotelService(
    IHotelService inner,
    IEnumerable<IValidator<CreateHotelRequest>> createValidators,
    IEnumerable<IValidator<UpdateHotelRequest>> updateValidators,
    IEnumerable<IValidator<GetHotelsRequest>> getHotelsValidators,
    IEnumerable<IValidator<UploadHotelImageRequest>> uploadImageValidators) : IHotelService
{
    public async Task<Result<HotelResponse>> CreateAsync(CreateHotelRequest request, Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await  inner.CreateAsync(request, ownerId, cancellationToken);
    }

    public async Task<Result<HotelResponse>> GetByIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
        => await inner.GetByIdAsync(hotelId, cancellationToken);

    public async Task<Result<PagedResult<HotelSummaryResponse>>> GetPagedAsync(GetHotelsRequest request, bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        await getHotelsValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await  inner.GetPagedAsync(request, isAdmin, cancellationToken);
    }

    public async Task<Result<HotelResponse>> UpdateAsync(Guid hotelId, UpdateHotelRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(hotelId, request, cancellationToken);
    }

    public async Task<Result<bool>> DeleteAsync(Guid hotelId, CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(hotelId, cancellationToken);

    public async Task<Result<HotelImageResponse>> InitiateUploadAsync(Guid hotelId, UploadHotelImageRequest request, CancellationToken cancellationToken = default)
    {
        await uploadImageValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await  inner.InitiateUploadAsync(hotelId, request, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<HotelImageResponse>>> GetImagesAsync(
        Guid hotelId, CancellationToken cancellationToken = default)
        => await inner.GetImagesAsync(hotelId, cancellationToken);
}