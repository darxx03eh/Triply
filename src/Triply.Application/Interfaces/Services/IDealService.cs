using Triply.Application.Common.Models;
using Triply.Application.DTOs.Deals;
using Triply.Application.Features.Deals.Commands.CreateDeal;
using Triply.Application.Features.Deals.Commands.UpdateDeal;
using Triply.Application.Features.Deals.Queries.GetDeals;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the deal operations.</summary>
public interface IDealService
{
    /// <summary>Creates a new deal.</summary>
    Task<Result<DealResponse>> CreateAsync(CreateDealRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets a paginated list of deals.</summary>
    Task<Result<PagedResult<DealResponse>>> GetPagedAsync(GetDealsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Gets the deal by its identifier.</summary>
    Task<Result<DealResponse>> GetByIdAsync(Guid dealId, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing deal.</summary>
    Task<Result<DealResponse>> UpdateAsync(Guid dealId, UpdateDealRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes the deal.</summary>
    Task<Result<bool>> DeleteAsync(Guid dealId, CancellationToken cancellationToken = default);
}