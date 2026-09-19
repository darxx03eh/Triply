using Triply.Api.Extensions;
using Triply.Application.Features.Deals.Commands.CreateDeal;
using Triply.Application.Features.Deals.Commands.UpdateDeal;
using Triply.Application.Features.Deals.Queries.GetDeals;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Infrastructure.Routes;

namespace Triply.Api.Endpoints;

public static class DealEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapDealEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Deals")
                .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

            group.MapPost(Router.DealRoutes.Create, async (
                    CreateDealRequest request, IDealService dealService, CancellationToken cancellationToken) =>
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
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict)
                .ProducesValidationProblem();

            group.MapGet(Router.DealRoutes.GetAll, async (
                    [AsParameters] GetDealsRequest request, IDealService dealService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await dealService.GetPagedAsync(request, cancellationToken);
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
                .Produces(StatusCodes.Status200OK);

            group.MapGet(Router.DealRoutes.GetById, async (
                    Guid id, IDealService dealService, CancellationToken cancellationToken) =>
                {
                    var result = await dealService.GetByIdAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetDealById")
                .WithDisplayName("Get Deal by id")
                .WithSummary("Retrieves a deal by its ID")
                .WithDescription("""
                                 Retrieves the details of a specific deal using its unique identifier.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut(Router.DealRoutes.Update, async (
                    Guid id, UpdateDealRequest request, IDealService dealService,
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
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict)
                .ProducesValidationProblem();

            group.MapDelete(Router.DealRoutes.Delete, async (
                    Guid id, IDealService dealService, CancellationToken cancellationToken) =>
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
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }
    }
}