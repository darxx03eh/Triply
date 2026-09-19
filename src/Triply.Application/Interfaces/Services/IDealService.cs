using Triply.Application.Common.Models;
using Triply.Application.DTOs.Deals;
using Triply.Application.Features.Deals.Commands.CreateDeal;
using Triply.Application.Features.Deals.Commands.UpdateDeal;
using Triply.Application.Features.Deals.Queries.GetDeals;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

public interface IDealService
{
    Task<Result<DealResponse>> CreateAsync(CreateDealRequest request, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<DealResponse>>> GetPagedAsync(GetDealsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<DealResponse>> GetByIdAsync(Guid dealId, CancellationToken cancellationToken = default);

    Task<Result<DealResponse>> UpdateAsync(Guid dealId, UpdateDealRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(Guid dealId, CancellationToken cancellationToken = default);
}