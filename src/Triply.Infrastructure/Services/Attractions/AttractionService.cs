using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Attractions;

/// <summary>Gets or sets the attraction service.</summary>
/// <summary>Implements the attraction operations.</summary>
public partial class AttractionService(
    IAttractionRepository attractionRepository,
    IHotelRepository hotelRepository,
    ILogger<AttractionService> logger) : IAttractionService{}