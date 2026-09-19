using Sieve.Models;
using Triply.Application.DTOs.Deals;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

public interface IDealRepository : IGenericRepository<Deal>
{
    Task<(List<Deal> Deals, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        CancellationToken cancellationToken = default);

    Task<List<FeaturedDealResponse>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default);

    Task<bool> IsDealOverlappingAsync(Guid roomId, DateTime startsAt, DateTime endsAt, Guid? excludeDealId = null,
        CancellationToken cancellationToken = default);
}