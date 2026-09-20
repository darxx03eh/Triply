using MessageQueue.IRabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Application.Options;

namespace Triply.Infrastructure.Services.Hotels;

/// <summary>Implements the hotel operations.</summary>
public partial class HotelService(
    IHotelRepository hotelRepository,
    IHotelImageRepository imageRepository,
    IOptions<ImageUploadOptions> uploadOptions,
    IMessagePublisher publisher,
    ICityRepository cityRepository,
    ILogger<HotelService> logger) : IHotelService
{
    
}