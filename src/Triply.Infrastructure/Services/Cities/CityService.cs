using MessageQueue.IRabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Application.Options;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService(
    ICityRepository cityRepository,
    IOptions<ImageUploadOptions> uploadOptions,
    IMessagePublisher publisher,
    ILogger<CityService> logger) : ICityService
{
    
}
