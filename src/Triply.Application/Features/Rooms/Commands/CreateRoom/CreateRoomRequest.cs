using System.Text.Json.Serialization;
using Triply.Domain.Enums.Rooms;

namespace Triply.Application.Features.Rooms.Commands.CreateRoom;

public class CreateRoomRequest
{
    public Guid HotelId { get; set; }
    public string Number { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; set; }
    public short AdultCapacity { get; set; } = 2;
    public short ChildCapacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Description { get; set; }
}
