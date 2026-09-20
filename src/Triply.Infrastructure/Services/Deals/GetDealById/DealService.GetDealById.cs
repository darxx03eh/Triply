using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Deals;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Deals;

public partial class DealService
{
    /// <summary>Gets the deal by its identifier.</summary>
    public async Task<Result<DealResponse>> GetByIdAsync(Guid dealId, CancellationToken cancellationToken = default)
    {
        var deal = await dealRepository.GetByIdAsync(dealId, cancellationToken);
        if (deal is null)
        {
            logger.LogWarning("Deal {DealId} was not found", dealId);
            return Result<DealResponse>.Failure(
                "DEAL_NOT_FOUND",
                $"The requested deal with id: {dealId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        return Result<DealResponse>.Success(deal.ToDealResponse(),
            success: new(
                "DEAL_FOUND", 
                $"The requested deal with id: {dealId.ToString()} was found."));
    }
}