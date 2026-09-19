using Triply.Application.Common.Models;
using Triply.Application.DTOs.Deals;
using Triply.Application.Extensions;
using Triply.Application.Features.Deals.Queries.GetDeals;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Deals;

public partial class DealService
{
    public async Task<Result<PagedResult<DealResponse>>> GetPagedAsync(GetDealsRequest request,
        CancellationToken cancellationToken = default)
    {
        var (deals, totalCount) = await dealRepository.GetPagedAsync(request, cancellationToken);

        var pagedResult = new PagedResult<DealResponse>
        {
            Items = deals.Select(d => d.ToDealResponse()).ToList(),
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        return Result<PagedResult<DealResponse>>.Success(pagedResult, success: deals.Count == 0
            ? new("DEALS_EMPTY", "No deals were found.")
            : new("DEALS_FOUND", "Deals were found successfully."));
    }
}