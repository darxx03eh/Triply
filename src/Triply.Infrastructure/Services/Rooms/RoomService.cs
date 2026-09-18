using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService(
    IRoomRepository roomRepository,
    IHotelRepository hotelRepository) : IRoomService
{
    
}
