using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Amenities;

public partial class AmenityService(
    IAmenityRepository amenityRepository,
    IHotelRepository hotelRepository) : IAmenityService
{
    
}
