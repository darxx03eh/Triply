using FluentValidation;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Deals;
using Triply.Application.Extensions;
using Triply.Application.Features.Deals.Commands.CreateDeal;
using Triply.Application.Features.Deals.Commands.UpdateDeal;
using Triply.Application.Features.Deals.Queries.GetDeals;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

/// <summary>Gets or sets the validated deal service.</summary>
/// <summary>Validates the requests before delegating to the deal service.</summary>
public class ValidatedDealService(
    IDealService inner,
    IEnumerable<IValidator<CreateDealRequest>> createValidators,
    IEnumerable<IValidator<UpdateDealRequest>> updateValidators,
    IEnumerable<IValidator<GetDealsRequest>> getDealsValidators) : IDealService
{
    /// <summary>Creates a new deal.</summary>
    public async Task<Result<DealResponse>> CreateAsync(CreateDealRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CreateAsync(request, cancellationToken);
    }

    /// <summary>Gets a paginated list of deals.</summary>
    public async Task<Result<PagedResult<DealResponse>>> GetPagedAsync(GetDealsRequest request,
        CancellationToken cancellationToken = default)
    {
        await getDealsValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.GetPagedAsync(request, cancellationToken);
    }

    /// <summary>Gets the deal by its identifier.</summary>
    public async Task<Result<DealResponse>> GetByIdAsync(Guid dealId, CancellationToken cancellationToken = default)
        => await inner.GetByIdAsync(dealId, cancellationToken);

    /// <summary>Updates an existing deal.</summary>
    public async Task<Result<DealResponse>> UpdateAsync(Guid dealId, UpdateDealRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(dealId, request, cancellationToken);
    }

    /// <summary>Deletes the deal.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid dealId, CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(dealId, cancellationToken);
}
