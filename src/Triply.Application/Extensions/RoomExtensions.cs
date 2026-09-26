using Triply.Application.DTOs.Rooms;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for room.</summary>
public static class RoomExtensions
{
    /// <summary>Maps the room to a room response.</summary>
    public static RoomResponse ToRoomResponse(this Room room, string hotelName, bool isBooked,
        IReadOnlyList<string> imageUrls)
        => new RoomResponse()
        {
            RoomId = room.RoomId,
            HotelId = room.HotelId,
            HotelName = hotelName,
            Number = room.Number,
            RoomType = room.RoomType,
            AdultCapacity = room.AdultCapacity,
            ChildCapacity = room.ChildCapacity,
            PricePerNight = room.PricePerNight,
            IsAvailable = room.IsAvailable,
            Description = room.Description,
            IsDeleted = room.IsDeleted,
            CreatedAt = room.CreatedAt,
            ModifiedAt = room.ModifiedAt,
            RowVersion = room.RowVersion,
            IsBooked = isBooked,
            ImageUrls = imageUrls
        };

    /// <summary>Maps the Create room request to a room.</summary>
    public static Room ToRoom(this CreateRoomRequest request)
        => new Room()
        {
            HotelId = request.HotelId,
            Number = request.Number.Trim(),
            RoomType = request.RoomType,
            AdultCapacity = request.AdultCapacity,
            ChildCapacity = request.ChildCapacity,
            PricePerNight = request.PricePerNight,
            IsAvailable = request.IsAvailable,
            Description = request.Description
        };
    /// <summary>Maps the hotel to a hotel image response.</summary>
    public static RoomImageResponse ToRoomImageResponse(this RoomImage image)
        => new RoomImageResponse
        {
            ImageId = image.ImageId,
            RoomId = image.RoomId,
            Url = image.Url,
            DisplayOrder = image.DisplayOrder,
            Status = image.Status
        };
}
