using Triply.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Application.Features.Cities.Commands.UpdateCity;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Infrastructure.Routes;

namespace Triply.Api.Endpoints;

public static class CityEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapCityEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Cities");
            
            group.MapPost(Router.CityRoutes.Create, async (
                    CreateCityRequest request, 
                    ICityService cityService, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await cityService.CreateAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("CreateCity")
                .WithDisplayName("Create City")
                .WithSummary("Create a new city")
                .WithDescription("""
                                 Creates a new city using the provided city information.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem();

            group.MapGet(Router.CityRoutes.GetAll, async (
                    [AsParameters] GetCitiesRequest request, 
                    [FromServices] ICurrentUserAccessor currentUser,
                    ICityService cityService, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await cityService.GetPagedAsync(request, currentUser.IsAdmin, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetAllCities")
                .WithDisplayName("Get All Cities")
                .WithSummary("Retrieves a paginated list of cities")
                .WithDescription("""
                                 Retrieves a paginated list of cities based on the provided
                                 pagination and filtering parameters.
                                 """)
                .Produces(StatusCodes.Status200OK);

            group.MapGet(Router.CityRoutes.GetById, async (
                    Guid id, ICityService cityService, CancellationToken cancellationToken) =>
                {
                    var result = await cityService.GetByIdAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("GetCityById")
                .WithDisplayName("Get city by id")
                .WithSummary("Retrieves a city by its ID")
                .WithDescription("""
                                 Retrieves the details of a specific city using its unique identifier.
                                 Returns a not found response if the city does not exist.
                                 """)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut(Router.CityRoutes.Update, async (
                    Guid id, UpdateCityRequest request, ICityService cityService, CancellationToken cancellationToken) =>
                {
                    request.CityId = id;
                    var result = await cityService.UpdateAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("UpdateCity")
                .WithDisplayName("Update City")
                .WithSummary("Updates an existing city")
                .WithDescription("""
                                 Updates the information of an existing city using its unique identifier.
                                 This endpoint is restricted to users with the Admin role.
                                 Returns a conflict response if the updated city information conflicts
                                 with an existing city.
                                 """)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapDelete(Router.CityRoutes.Delete, async (
                    Guid id, ICityService cityService, CancellationToken cancellationToken) =>
                {
                    var result = await cityService.DeleteAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("DeleteCity")
                .WithDisplayName("Delete City")
                .WithSummary("Deletes a city")
                .WithDescription("""
                                 Deletes an existing city using its unique identifier.
                                 This endpoint is restricted to users with the Admin role.
                                 Returns a not found response if the city does not exist.
                                 """)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
        }
    }
}