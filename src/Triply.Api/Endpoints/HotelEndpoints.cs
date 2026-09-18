using System.Security.Claims;
using Triply.Api.Extensions;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Application.Features.Hotels.Commands.UpdateHotel;
using Triply.Application.Features.Hotels.Queries.GetHotels;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Infrastructure.Routes;

namespace Triply.Api.Endpoints;

public static class HotelEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapHotelEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Hotels");
            
            group.MapPost(Router.HotelRoutes.Create, async (
                    CreateHotelRequest request,
                    ICurrentUserAccessor user,
                    IHotelService hotelService, CancellationToken cancellationToken) =>
                {
                    var result = await hotelService.CreateAsync(request, user.UserId, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("CreateHotel")
                .WithDisplayName("Create Hotel")
                .WithSummary("Creates a new hotel")
                .WithDescription("""
                                 Creates a new hotel using the provided hotel details. 
                                 The authenticated administrator is assigned as the creator 
                                 of the hotel.
                                 """)
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem();

            group.MapGet(Router.HotelRoutes.GetAll, async (
                    [AsParameters] GetHotelsRequest request, 
                    ICurrentUserAccessor user,
                    IHotelService hotelService, CancellationToken cancellationToken) =>
                {
                    var result = await hotelService.GetPagedAsync(request, user.IsAdmin, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetAllHotels")
                .WithDisplayName("Get All Hotels")
                .WithSummary("Retrieves a paginated list of hotels")
                .WithDescription("""
                                 Retrieves a paginated list of hotels based on the provided 
                                 filtering, sorting, and pagination parameters. 
                                 The returned data may vary depending on whether the current 
                                 user has administrator privileges.
                                 """)
                .Produces(StatusCodes.Status200OK);

            group.MapGet(Router.HotelRoutes.GetById, async (
                    Guid id, IHotelService hotelService, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await hotelService.GetByIdAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("GetHotelById")
                .WithDisplayName("Get Hotel by id")
                .WithSummary("Retrieves a hotel by its ID")
                .WithDescription("""
                                 Retrieves the details of a specific hotel using its unique 
                                 identifier. Returns the hotel details when the hotel exists.
                                 """)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut(Router.HotelRoutes.Update, async (
                    Guid id, UpdateHotelRequest request, 
                    IHotelService hotelService, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await hotelService.UpdateAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("UpdateHotel")
                .WithDisplayName("Update Hotel")
                .WithSummary("Updates an existing hotel")
                .WithDescription("""
                                 Updates the details of an existing hotel using its unique 
                                 identifier and the provided update information. 
                                 Returns a conflict response when the requested update 
                                 violates a uniqueness or business constraint.
                                 """)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status409Conflict);

            group.MapDelete(Router.HotelRoutes.Delete, async (
                    Guid id, 
                    IHotelService hotelService, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await hotelService.DeleteAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("DeleteHotel")
                .WithDisplayName("Delete Hotel")
                .WithSummary("Deletes a hotel")
                .WithDescription("""
                                 Deletes an existing hotel using its unique identifier. 
                                 Returns a not found response when the specified hotel 
                                 does not exist.
                                 """)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
        }
    }
}