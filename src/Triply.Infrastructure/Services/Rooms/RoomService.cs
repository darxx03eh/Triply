using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Rooms;

/// <summary>Implements the room operations.</summary>
public partial class RoomService(
    IRoomRepository roomRepository,
    IHotelRepository hotelRepository,
    ILogger<RoomService> logger) : IRoomService
{
    
}
