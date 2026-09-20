using System.Text.Json.Serialization;
using Triply.Domain.Enums.Rooms;

namespace Triply.Application.Features.Rooms.Commands.UpdateRoom;

/// <summary>Request payload used to update room.</summary>
public class UpdateRoomRequest
{
    /// <summary>Gets or sets the identifier of the room.</summary>
    [JsonIgnore]
    public Guid RoomId { get; set; }
    /// <summary>Gets or sets the number.</summary>
    public string Number { get; set; }
    /// <summary>Gets or sets the room type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; set; }
    /// <summary>Gets or sets the adult capacity.</summary>
    public short AdultCapacity { get; set; }
    /// <summary>Gets or sets the child capacity.</summary>
    public short ChildCapacity { get; set; }
    /// <summary>Gets or sets the price per night.</summary>
    public decimal PricePerNight { get; set; }
    /// <summary>Gets or sets whether the update room is available.</summary>
    public bool IsAvailable { get; set; }
    /// <summary>Gets or sets the description.</summary>
    public string? Description { get; set; }
    /// <summary>Gets or sets the row version.</summary>
    public byte[] RowVersion { get; set; }
}
