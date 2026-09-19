using Triply.Application.DTOs.Deals;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Deals;

public partial class DealService
{
    public async Task<Result<DealResponse>> GetByIdAsync(Guid dealId, CancellationToken cancellationToken = default)
    {
        var deal = await dealRepository.GetByIdAsync(dealId, cancellationToken);
        if (deal is null)
            return Result<DealResponse>.Failure(
                "DEAL_NOT_FOUND",
                $"The requested deal with id: {dealId.ToString()} was not found.",
                ResultErrorType.NotFound);

        return Result<DealResponse>.Success(deal.ToDealResponse(),
            success: new("DEAL_FOUND", $"The requested deal with id: {dealId.ToString()} was found."));
    }
}