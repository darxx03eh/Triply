using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Deals;

/// <summary>Implements the deal operations.</summary>
public partial class DealService(
    IDealRepository dealRepository,
    IRoomRepository roomRepository,
    ILogger<DealService> logger) : IDealService{}