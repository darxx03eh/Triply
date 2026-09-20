using System.Text.Json.Serialization;
using Triply.Domain.Enums.Rooms;

namespace Triply.Application.DTOs.Rooms;

/// <summary>Response returned for the room.</summary>
public record RoomResponse
{
    /// <summary>Gets the identifier of the room.</summary>
    public Guid RoomId { get; init; }
    /// <summary>Gets the identifier of the hotel.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the hotel name.</summary>
    public string HotelName { get; init; }
    /// <summary>Gets the number.</summary>
    public string Number { get; init; }
    /// <summary>Gets the room type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; init; }
    /// <summary>Gets the adult capacity.</summary>
    public short AdultCapacity { get; init; }
    /// <summary>Gets the child capacity.</summary>
    public short ChildCapacity { get; init; }
    /// <summary>Gets the price per night.</summary>
    public decimal PricePerNight { get; init; }
    /// <summary>Gets whether the room is available.</summary>
    public bool IsAvailable { get; init; }
    /// <summary>Gets the description.</summary>
    public string? Description { get; init; }
    /// <summary>Gets whether the room is deleted.</summary>
    public bool IsDeleted { get; init; }
    /// <summary>Gets when the room was created.</summary>
    public DateTime CreatedAt { get; init; }
    /// <summary>Gets when the room was modified.</summary>
    public DateTime? ModifiedAt { get; init; }
    /// <summary>Gets the row version.</summary>
    public byte[] RowVersion { get; init; } = [];
}
