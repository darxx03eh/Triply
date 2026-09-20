using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Deals;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Home;

/// <summary>Implements the home operations.</summary>
public partial class HomeService
{
    /// <summary>Gets the featured deals.</summary>
    public async Task<Result<IReadOnlyList<FeaturedDealResponse>>> GetFeaturedDealsAsync(int? count,
        CancellationToken cancellationToken = default)
    {
        var deals = await dealRepository
            .GetFeaturedAsync(Math.Clamp(count ?? 5, 1, 20), cancellationToken);
        logger.LogDebug("Returned {Count} featured deals", deals.Count);

        return Result<IReadOnlyList<FeaturedDealResponse>>.Success(deals, success: deals.Count == 0
            ? new("FEATURED_DEALS_EMPTY", "There are no featured deals right now.")
            : new("FEATURED_DEALS_FOUND", "Featured deals were retrieved successfully."));
    }
}