using Triply.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Application.Features.Cities.Commands.UpdateCity;
using Triply.Application.Features.Cities.Commands.UploadCityThumbnail;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Infrastructure.Routes;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Cities;

namespace Triply.Api.Endpoints;

/// <summary>Maps the city endpoints.</summary>
public static class CityEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the city endpoints.</summary>
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
                .Produces<ApiResponse<CityResponse>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity);

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
                .Produces<ApiResponse<PagedResult<CityResponse>>>(StatusCodes.Status200OK);

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
                .Produces<ApiResponse<CityResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

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
                .Produces<ApiResponse<CityResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict);

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
                .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapPost(Router.CityRoutes.Thumbnail, async (
                    Guid id, [FromForm] UploadCityThumbnailRequest request,
                    ICityService cityService, CancellationToken cancellationToken) =>
                {
                    var result = await cityService.UploadThumbnailAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .DisableAntiforgery()
                .WithName("UploadCityThumbnail")
                .WithDisplayName("Upload City Thumbnail")
                .WithSummary("Queues a thumbnail upload for a city")
                .WithDescription("""
                                 Accepts a JPG, PNG or WEBP image as multipart/form-data and queues it
                                 for background upload to cloud storage. Once uploaded it replaces the
                                 current thumbnail of the city, which is shown in trending destinations.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Accepts<UploadCityThumbnailRequest>("multipart/form-data")
                .Produces<ApiResponse<CityResponse>>(StatusCodes.Status202Accepted)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity);

            group.MapDelete(Router.CityRoutes.Thumbnail, async (
                    Guid id, ICityService cityService, CancellationToken cancellationToken) =>
                {
                    var result = await cityService.DeleteThumbnailAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("DeleteCityThumbnail")
                .WithDisplayName("Delete City Thumbnail")
                .WithSummary("Deletes the thumbnail of a city")
                .WithDescription("""
                                 Removes the thumbnail of a city and queues the removal of the stored
                                 file from cloud storage.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
        }
    }
}