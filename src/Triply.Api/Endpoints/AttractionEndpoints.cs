using Triply.Api.Extensions;
using Triply.Application.Features.Attractions.Commands.CreateAttraction;
using Triply.Application.Features.Attractions.Commands.UpdateAttraction;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Infrastructure.Routes;
using Triply.Api.Responses;
using Triply.Application.DTOs.Attractions;

namespace Triply.Api.Endpoints;

/// <summary>Maps the attraction endpoints.</summary>
public static class AttractionEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the attraction endpoints.</summary>
        public void MapAttractionEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Attractions");

            group.MapGet(Router.HotelRoutes.Attractions, async (
                    Guid id, IAttractionService attractionService, CancellationToken cancellationToken) =>
                {
                    var result = await attractionService.GetHotelAttractionsAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetHotelAttractions")
                .WithDisplayName("Get Hotel Attractions")
                .WithSummary("Retrieves the attractions near a hotel")
                .WithDescription("""
                                 Retrieves the attractions near a specific hotel ordered by distance.
                                 Returns a not found response when the hotel does not exist.
                                 """)
                .Produces<ApiResponse<IReadOnlyList<AttractionResponse>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapPost(Router.HotelRoutes.Attractions, async (
                    Guid id,
                    CreateAttractionRequest request,
                    IAttractionService attractionService,
                    CancellationToken cancellationToken) =>
                {
                    request.HotelId = id;
                    var result = await attractionService.CreateAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("CreateAttraction")
                .WithDisplayName("Create Attraction")
                .WithSummary("Adds a nearby attraction to a hotel")
                .WithDescription("""
                                 Adds a nearby attraction (name, category and distance in km) to a hotel.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces<ApiResponse<AttractionResponse>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity);

            group.MapPut(Router.AttractionRoutes.Update, async (
                    Guid id,
                    UpdateAttractionRequest request,
                    IAttractionService attractionService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await attractionService.UpdateAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("UpdateAttraction")
                .WithDisplayName("Update Attraction")
                .WithSummary("Updates a nearby attraction")
                .WithDescription("""
                                 Updates the name, category and distance of an attraction.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces<ApiResponse<AttractionResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapDelete(Router.AttractionRoutes.Delete, async (
                    Guid id, IAttractionService attractionService, CancellationToken cancellationToken) =>
                {
                    var result = await attractionService.DeleteAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("DeleteAttraction")
                .WithDisplayName("Delete Attraction")
                .WithSummary("Deletes a nearby attraction")
                .WithDescription("""
                                 Deletes an attraction from a hotel.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
        }
    }
}