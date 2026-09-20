using Triply.Application.DTOs.Rooms;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for room.</summary>
public static class RoomExtensions
{
    /// <summary>Maps the room to a room response.</summary>
    public static RoomResponse ToRoomResponse(this Room room, string hotelName)
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
        };
}
