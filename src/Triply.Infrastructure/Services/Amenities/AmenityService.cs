using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Amenities;

/// <summary>Gets or sets the amenity service.</summary>
/// <summary>Implements the amenity operations.</summary>
public partial class AmenityService(
    IAmenityRepository amenityRepository,
    IHotelRepository hotelRepository,
    ILogger<AmenityService> logger) : IAmenityService
{
    
}
