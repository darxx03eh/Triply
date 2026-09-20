using Microsoft.Extensions.Logging;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Deals;

public partial class DealService
{
    /// <summary>Deletes the deal.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid dealId, CancellationToken cancellationToken = default)
    {
        var deal = await dealRepository.GetByIdAsync(dealId, cancellationToken);
        if (deal is null)
        {
            logger.LogWarning("Delete deal failed: deal {DealId} was not found", dealId);
            return Result<bool>.Failure(
                "DEAL_NOT_FOUND",
                $"The requested deal with id: {dealId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        await dealRepository.DeleteAsync(deal);
        await dealRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Deal {DealId} ({DealTitle}) on room {RoomId} deleted", dealId, deal.Title, deal.RoomId);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ("DEAL_DELETED", "Deal deleted successfully."));
    }
}