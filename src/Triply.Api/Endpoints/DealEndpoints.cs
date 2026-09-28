using Triply.Api.Extensions;
using Triply.Application.Features.Deals.Commands.CreateDeal;
using Triply.Application.Features.Deals.Commands.UpdateDeal;
using Triply.Application.Features.Deals.Queries.GetDeals;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Infrastructure.Routes;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Deals;
using Triply.Domain.Results;
using Triply.Infrastructure.Caching;

namespace Triply.Api.Endpoints;

/// <summary>Maps the deal endpoints.</summary>
public static class DealEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the deal endpoints.</summary>
        public void MapDealEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Deals")
                .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

            group.MapPost(Router.DealRoutes.Create, async (
                    CreateDealRequest request,
                    IDealService dealService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await dealService.CreateAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("CreateDeal")
                .WithDisplayName("Create Deal")
                .WithSummary("Creates a new deal on a room")
                .WithDescription("""
                                 Creates a discount deal on a specific room for a period of time.
                                 A room can not have two deals in the same period.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .WithRateLimit(
                    "create-deal",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Deal.CreateRateLimited)
                .Produces<ApiResponse<DealResponse>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity);

            group.MapGet(Router.DealRoutes.GetAll, async (
                    [AsParameters] GetDealsRequest request,
                    IDealService dealService,
                    ICacheService cache,
                    CancellationToken cancellationToken) =>
                {
                    var result = await cache.GetOrCreateAsync(CacheGroups.Deals,
                        CacheKeyBuilder.BuildPagedSieveKey(request.Filters, request.Sorts, 
                            request.Page, request.PageSize, isAdmin: true), TimeSpan.FromMinutes(5),
                        ct => dealService.GetPagedAsync(request, ct), cancellationToken);

                    return result.ToMinimalApiResult();
                })
                .WithName("GetDeals")
                .WithDisplayName("Get Deals")
                .WithSummary("Retrieves a paginated list of deals")
                .WithDescription("""
                                 Retrieves a paginated list of deals, supports filtering and sorting
                                 (filters=featured==true, sorts=-discount).
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .WithRateLimit(
                    "get-deals",
                    60,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Deal.GetAllRateLimited)
                .Produces<ApiResponse<PagedResult<DealResponse>>>(
                    StatusCodes.Status200OK);

            group.MapGet(Router.DealRoutes.GetById, async (
                    Guid id,
                    IDealService dealService,
                    ICacheService cache,
                    CancellationToken cancellationToken) =>
                {
                    var result = await cache.GetOrCreateAsync(CacheGroups.Deals, 
                        CacheKeyBuilder.BuildKey(new { id }),
                        TimeSpan.FromMinutes(5), ct => dealService.GetByIdAsync(id, ct), cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetDealById")
                .WithDisplayName("Get Deal by id")
                .WithSummary("Retrieves a deal by its ID")
                .WithDescription("""
                                 Retrieves the details of a specific deal using its unique identifier.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .WithRateLimit(
                    "get-deal-by-id",
                    60,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Deal.GetByIdRateLimited)
                .Produces<ApiResponse<DealResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapPut(Router.DealRoutes.Update, async (
                    Guid id,
                    UpdateDealRequest request,
                    IDealService dealService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await dealService.UpdateAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("UpdateDeal")
                .WithDisplayName("Update Deal")
                .WithSummary("Updates an existing deal")
                .WithDescription("""
                                 Updates the title, discount, period and featured flag of a deal.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .WithRateLimit(
                    "update-deal",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Deal.UpdateRateLimited)
                .Produces<ApiResponse<DealResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity);

            group.MapDelete(Router.DealRoutes.Delete, async (
                    Guid id,
                    IDealService dealService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await dealService.DeleteAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("DeleteDeal")
                .WithDisplayName("Delete Deal")
                .WithSummary("Deletes a deal")
                .WithDescription("""
                                 Deletes a deal permanently.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .WithRateLimit(
                    "delete-deal",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Deal.DeleteRateLimited)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
        }
    }
}
