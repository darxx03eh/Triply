using FluentValidation;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Application.Features.Rooms.Queries.GetRooms;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

public class ValidatedRoomService(
    IRoomService inner,
    IEnumerable<IValidator<CreateRoomRequest>> createValidators,
    IEnumerable<IValidator<UpdateRoomRequest>> updateValidators,
    IEnumerable<IValidator<GetRoomsRequest>> getRoomsValidators) : IRoomService
{
    public async Task<Result<RoomResponse>> CreateAsync(CreateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CreateAsync(request, cancellationToken);
    }

    public async Task<Result<RoomResponse>> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
        => await inner.GetByIdAsync(roomId, cancellationToken);

    public async Task<Result<PagedResult<RoomResponse>>> GetPagedAsync(GetRoomsRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        await getRoomsValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.GetPagedAsync(request, isAdmin, cancellationToken);
    }

    public async Task<Result<PagedResult<RoomResponse>>> GetHotelRoomsAsync(Guid hotelId, GetRoomsRequest request,
        CancellationToken cancellationToken = default)
    {
        await getRoomsValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.GetHotelRoomsAsync(hotelId, request, cancellationToken);
    }

    public async Task<Result<RoomResponse>> UpdateAsync(Guid roomId, UpdateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(roomId, request, cancellationToken);
    }

    public async Task<Result<bool>> DeleteAsync(Guid roomId, CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(roomId, cancellationToken);
}
