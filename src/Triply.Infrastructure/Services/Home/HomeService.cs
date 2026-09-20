using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Home;

/// <summary>Implements the home operations.</summary>
public partial class HomeService(
    IRecentVisitRepository recentVisitRepository,
    IDealRepository dealRepository,
    ILogger<HomeService> logger) : IHomeService {}