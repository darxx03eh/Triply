using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Attractions;

public partial class AttractionService(
    IAttractionRepository attractionRepository,
    IHotelRepository hotelRepository) : IAttractionService{}