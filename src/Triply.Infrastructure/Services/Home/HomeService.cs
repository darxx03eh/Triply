using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Home;

public partial class HomeService(
    IRecentVisitRepository recentVisitRepository,
    IDealRepository dealRepository) : IHomeService {}