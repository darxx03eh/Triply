using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Deals;

public partial class DealService(
    IDealRepository dealRepository,
    IRoomRepository roomRepository) : IDealService{}