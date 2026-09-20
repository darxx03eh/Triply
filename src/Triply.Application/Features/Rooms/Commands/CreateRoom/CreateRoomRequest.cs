using System.Text.Json.Serialization;
using Triply.Domain.Enums.Rooms;

namespace Triply.Application.Features.Rooms.Commands.CreateRoom;

/// <summary>Request payload used to create room.</summary>
public class CreateRoomRequest
{
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    public Guid HotelId { get; set; }
    /// <summary>Gets or sets the number.</summary>
    public string Number { get; set; }
    /// <summary>Gets or sets the room type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; set; }
    /// <summary>Gets or sets the adult capacity.</summary>
    public short AdultCapacity { get; set; } = 2;
    /// <summary>Gets or sets the child capacity.</summary>
    public short ChildCapacity { get; set; }
    /// <summary>Gets or sets the price per night.</summary>
    public decimal PricePerNight { get; set; }
    /// <summary>Gets or sets whether the create room is available.</summary>
    public bool IsAvailable { get; set; } = true;
    /// <summary>Gets or sets the description.</summary>
    public string? Description { get; set; }
}
