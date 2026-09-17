using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService(ICityRepository cityRepository) : ICityService
{
    
}