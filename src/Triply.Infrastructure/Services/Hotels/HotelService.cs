using MessageQueue.IRabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Application.Options;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService(
    IHotelRepository hotelRepository,
    IHotelImageRepository imageRepository,
    IOptions<ImageUploadOptions> uploadOptions,
    IMessagePublisher publisher,
    ICityRepository cityRepository,
    ILogger<HotelService> logger) : IHotelService
{
    
}