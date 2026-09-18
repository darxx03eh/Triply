using System.Text.Json.Serialization;
using Triply.Domain.Enums.Rooms;

namespace Triply.Application.Features.Rooms.Commands.UpdateRoom;

public class UpdateRoomRequest
{
    [JsonIgnore]
    public Guid RoomId { get; set; }
    public string Number { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; set; }
    public short AdultCapacity { get; set; }
    public short ChildCapacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
    public string? Description { get; set; }
    public byte[] RowVersion { get; set; }
}
