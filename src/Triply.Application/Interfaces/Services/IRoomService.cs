using Triply.Application.Common.Models;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Features.Hotels.Commands.UploadImage;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Application.Features.Rooms.Commands.UploadImage;
using Triply.Application.Features.Rooms.Queries.GetRooms;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the room operations.</summary>
public interface IRoomService
{
    /// <summary>Creates a new room.</summary>
    Task<Result<RoomResponse>> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken = default);
    /// <summary>Gets the room by its identifier.</summary>
    Task<Result<RoomResponse>> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>Gets a paginated list of rooms.</summary>
    Task<Result<PagedResult<RoomResponse>>> GetPagedAsync(GetRoomsRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the hotel rooms.</summary>
    Task<Result<PagedResult<RoomResponse>>> GetHotelRoomsAsync(Guid hotelId, GetRoomsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Updates an existing room.</summary>
    Task<Result<RoomResponse>> UpdateAsync(Guid roomId, UpdateRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes the room.</summary>
    Task<Result<bool>> DeleteAsync(Guid roomId, CancellationToken cancellationToken = default);
    /// <summary>Initiates the upload.</summary>
    Task<Result<RoomImageResponse>> InitiateUploadAsync(
        Guid roomId, UploadRoomImageRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the images.</summary>
    Task<Result<IReadOnlyList<RoomImageResponse>>> GetImagesAsync(
        Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>Deletes the image.</summary>
    Task<Result<bool>> DeleteImageAsync(Guid roomId, Guid imageId, CancellationToken cancellationToken = default);
}
