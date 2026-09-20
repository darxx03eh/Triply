using Sieve.Models;
using Triply.Application.DTOs.Deals;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for deal.</summary>
public interface IDealRepository : IGenericRepository<Deal>
{
    /// <summary>Gets a paginated list of deals.</summary>
    Task<(List<Deal> Deals, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the featured.</summary>
    Task<List<FeaturedDealResponse>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>Checks whether the deal overlapping.</summary>
    Task<bool> IsDealOverlappingAsync(Guid roomId, DateTime startsAt, DateTime endsAt, Guid? excludeDealId = null,
        CancellationToken cancellationToken = default);
}