using MessageQueue.IRabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Application.Options;

namespace Triply.Infrastructure.Services.Rooms;

/// <summary>Implements the room operations.</summary>
public partial class RoomService(
    IRoomRepository roomRepository,
    IHotelRepository hotelRepository,
    IRoomImageRepository imageRepository,
    IMessagePublisher publisher,
    IOptions<ImageUploadOptions> uploadOptions,
    ILogger<RoomService> logger) : IRoomService
{
    
}
