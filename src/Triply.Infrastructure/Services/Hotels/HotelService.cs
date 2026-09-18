using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService(
    IHotelRepository hotelRepository,
    ICityRepository cityRepository) : IHotelService
{
    
}