using Triply.Api.Extensions;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Application.Features.Amenities.Commands.UpdateAmenity;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Infrastructure.Routes;
using Triply.Api.Responses;
using Triply.Application.DTOs.Amenities;

namespace Triply.Api.Endpoints;

/// <summary>Maps the amenity endpoints.</summary>
public static class AmenityEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the amenity endpoints.</summary>
        public void MapAmenityEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Amenities");

            group.MapPost(Router.AmenityRoutes.Create, async (
                    CreateAmenityRequest request,
                    IAmenityService amenityService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await amenityService.CreateAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("CreateAmenity")
                .WithDisplayName("Create Amenity")
                .WithSummary("Creates a new amenity")
                .WithDescription("""
                                 Creates a new amenity (e.g. Free WiFi, Swimming Pool) that can be
                                 assigned to hotels. Amenity names must be unique.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces<ApiResponse<AmenityResponse>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity);

            group.MapGet(Router.AmenityRoutes.GetAll, async (
                    IAmenityService amenityService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await amenityService.GetAllAsync(cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetAllAmenities")
                .WithDisplayName("Get All Amenities")
                .WithSummary("Retrieves all amenities")
                .WithDescription("""
                                 Retrieves all available amenities ordered by name.
                                 Used to build the amenities filter in the search page.
                                 """)
                .Produces<ApiResponse<IReadOnlyList<AmenityResponse>>>(StatusCodes.Status200OK);

            group.MapGet(Router.AmenityRoutes.GetById, async (
                    Guid id, IAmenityService amenityService, CancellationToken cancellationToken) =>
                {
                    var result = await amenityService.GetByIdAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("GetAmenityById")
                .WithDisplayName("Get Amenity by id")
                .WithSummary("Retrieves an amenity by its ID")
                .WithDescription("""
                                 Retrieves the details of a specific amenity using its unique identifier.
                                 Returns a not found response if the amenity does not exist.
                                 """)
                .Produces<ApiResponse<AmenityResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapPut(Router.AmenityRoutes.Update, async (
                    Guid id, UpdateAmenityRequest request,
                    IAmenityService amenityService,
                    CancellationToken cancellationToken) =>
                {
                    request.AmenityId = id;
                    var result = await amenityService.UpdateAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("UpdateAmenity")
                .WithDisplayName("Update Amenity")
                .WithSummary("Updates an existing amenity")
                .WithDescription("""
                                 Renames an existing amenity using its unique identifier.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces<ApiResponse<AmenityResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapDelete(Router.AmenityRoutes.Delete, async (
                    Guid id, IAmenityService amenityService, CancellationToken cancellationToken) =>
                {
                    var result = await amenityService.DeleteAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("DeleteAmenity")
                .WithDisplayName("Delete Amenity")
                .WithSummary("Deletes an amenity")
                .WithDescription("""
                                 Deletes an amenity and removes it from every hotel it was assigned to.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapGet(Router.HotelRoutes.Amenities, async (
                    Guid id, IAmenityService amenityService, CancellationToken cancellationToken) =>
                {
                    var result = await amenityService.GetHotelAmenitiesAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithTags("Hotels")
                .WithName("GetHotelAmenities")
                .WithDisplayName("Get Hotel Amenities")
                .WithSummary("Retrieves the amenities of a hotel")
                .WithDescription("""
                                 Retrieves the amenities offered by a specific hotel ordered by name.
                                 Returns a not found response when the hotel does not exist.
                                 """)
                .Produces<ApiResponse<IReadOnlyList<AmenityResponse>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapPut(Router.HotelRoutes.Amenities, async (
                    Guid id, SetHotelAmenitiesRequest request,
                    IAmenityService amenityService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await amenityService.SetHotelAmenitiesAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithTags("Hotels")
                .WithName("SetHotelAmenities")
                .WithDisplayName("Set Hotel Amenities")
                .WithSummary("Replaces the amenities of a hotel")
                .WithDescription("""
                                 Replaces the amenities of a hotel with the provided list of amenity ids.
                                 Amenities missing from the list are removed and new ones are added.
                                 Send an empty list to remove all amenities.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces<ApiResponse<IReadOnlyList<AmenityResponse>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity);
        }
    }
}
