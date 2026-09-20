using Microsoft.AspNetCore.Mvc;
using Triply.Api.Extensions;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Application.Features.Rooms.Queries.GetRooms;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Infrastructure.Routes;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Rooms;

namespace Triply.Api.Endpoints;

/// <summary>Maps the room endpoints.</summary>
public static class RoomEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the room endpoints.</summary>
        public void MapRoomEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Rooms");

            group.MapPost(Router.RoomRoutes.Create, async (
                    CreateRoomRequest request,
                    IRoomService roomService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await roomService.CreateAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("CreateRoom")
                .WithDisplayName("Create Room")
                .WithSummary("Creates a new room")
                .WithDescription("""
                                 Creates a new room in the specified hotel using the provided room details.
                                 The room number must be unique within the hotel.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces<ApiResponse<RoomResponse>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity);

            group.MapGet(Router.RoomRoutes.GetAll, async (
                    [AsParameters] GetRoomsRequest request,
                    ICurrentUserAccessor user,
                    IRoomService roomService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await roomService.GetPagedAsync(request, user.IsAdmin, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetAllRooms")
                .WithDisplayName("Get All Rooms")
                .WithSummary("Retrieves a paginated list of rooms")
                .WithDescription("""
                                 Retrieves a paginated list of rooms across all hotels based on the provided
                                 filtering, sorting, and pagination parameters (e.g. filters=HotelId==...).
                                 Deleted rooms are included for administrators.
                                 """)
                .Produces<ApiResponse<PagedResult<RoomResponse>>>(StatusCodes.Status200OK);

            group.MapGet(Router.HotelRoutes.GetRooms, async (
                    Guid id,
                    [AsParameters] GetRoomsRequest request,
                    IRoomService roomService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await roomService.GetHotelRoomsAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithTags("Hotels")
                .WithName("GetHotelRooms")
                .WithDisplayName("Get Hotel Rooms")
                .WithSummary("Retrieves the rooms of a hotel")
                .WithDescription("""
                                 Retrieves a paginated list of the rooms of a specific hotel with their
                                 type, capacity, price per night and availability.
                                 Returns a not found response when the hotel does not exist.
                                 """)
                .Produces<ApiResponse<PagedResult<RoomResponse>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapGet(Router.RoomRoutes.GetById, async (
                    Guid id, IRoomService roomService, CancellationToken cancellationToken) =>
                {
                    var result = await roomService.GetByIdAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("GetRoomById")
                .WithDisplayName("Get Room by id")
                .WithSummary("Retrieves a room by its ID")
                .WithDescription("""
                                 Retrieves the details of a specific room using its unique identifier.
                                 Returns a not found response if the room does not exist.
                                 """)
                .Produces<ApiResponse<RoomResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapPut(Router.RoomRoutes.Update, async (
                    Guid id, UpdateRoomRequest request,
                    IRoomService roomService,
                    CancellationToken cancellationToken) =>
                {
                    request.RoomId = id;
                    var result = await roomService.UpdateAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("UpdateRoom")
                .WithDisplayName("Update Room")
                .WithSummary("Updates an existing room")
                .WithDescription("""
                                 Updates the details of an existing room using its unique identifier.
                                 Returns a conflict response when the room was modified by someone else.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces<ApiResponse<RoomResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict);

            group.MapDelete(Router.RoomRoutes.Delete, async (
                    Guid id, IRoomService roomService, CancellationToken cancellationToken) =>
                {
                    var result = await roomService.DeleteAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("DeleteRoom")
                .WithDisplayName("Delete Room")
                .WithSummary("Deletes a room")
                .WithDescription("""
                                 Deletes an existing room using its unique identifier.
                                 Returns a not found response when the specified room does not exist.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
        }
    }
}
