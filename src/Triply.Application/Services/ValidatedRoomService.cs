using FluentValidation;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Application.Features.Rooms.Commands.UploadImage;
using Triply.Application.Features.Rooms.Queries.GetRooms;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

/// <summary>Gets or sets the validated room service.</summary>
/// <summary>Validates the requests before delegating to the room service.</summary>
public class ValidatedRoomService(
    IRoomService inner,
    IEnumerable<IValidator<CreateRoomRequest>> createValidators,
    IEnumerable<IValidator<UpdateRoomRequest>> updateValidators,
    IEnumerable<IValidator<GetRoomsRequest>> getRoomsValidators,
    IEnumerable<IValidator<UploadRoomImageRequest>> uploadImageValidators) : IRoomService
{
    /// <summary>Creates a new room.</summary>
    public async Task<Result<RoomResponse>> CreateAsync(CreateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CreateAsync(request, cancellationToken);
    }

    /// <summary>Gets the room by its identifier.</summary>
    public async Task<Result<RoomResponse>> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
        => await inner.GetByIdAsync(roomId, cancellationToken);

    /// <summary>Gets a paginated list of rooms.</summary>
    public async Task<Result<PagedResult<RoomResponse>>> GetPagedAsync(GetRoomsRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        await getRoomsValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.GetPagedAsync(request, isAdmin, cancellationToken);
    }

    /// <summary>Gets the hotel rooms.</summary>
    public async Task<Result<PagedResult<RoomResponse>>> GetHotelRoomsAsync(Guid hotelId, GetRoomsRequest request,
        CancellationToken cancellationToken = default)
    {
        await getRoomsValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.GetHotelRoomsAsync(hotelId, request, cancellationToken);
    }

    /// <summary>Updates an existing room.</summary>
    public async Task<Result<RoomResponse>> UpdateAsync(Guid roomId, UpdateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(roomId, request, cancellationToken);
    }

    /// <summary>Deletes the room.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid roomId, CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(roomId, cancellationToken);

    public async Task<Result<RoomImageResponse>> InitiateUploadAsync(Guid roomId, UploadRoomImageRequest request,
        CancellationToken cancellationToken = default)
    {
        await uploadImageValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.InitiateUploadAsync(roomId, request, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<RoomImageResponse>>> GetImagesAsync(Guid roomId,
        CancellationToken cancellationToken = default)
    => await inner.GetImagesAsync(roomId, cancellationToken);

    public async Task<Result<bool>> DeleteImageAsync(Guid roomId, Guid imageId,
        CancellationToken cancellationToken = default)
    => await inner.DeleteImageAsync(roomId, imageId, cancellationToken);
}
