using Microsoft.AspNetCore.Mvc;
using Triply.Api.Extensions;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Application.Features.Rooms.Commands.UploadImage;
using Triply.Application.Features.Rooms.Queries.GetRooms;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Infrastructure.Routes;

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
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity)
                .WithRateLimit(
                    "create-room",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Room.CreateRateLimited);

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
                .Produces<ApiResponse<PagedResult<RoomResponse>>>(StatusCodes.Status200OK)
                .WithRateLimit(
                    "get-all-rooms",
                    60,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Room.GetAllRateLimited);

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
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .WithRateLimit(
                    "get-hotel-rooms",
                    60,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Room.GetHotelRoomsRateLimited);

            group.MapGet(Router.RoomRoutes.GetById, async (
                    Guid id,
                    IRoomService roomService,
                    CancellationToken cancellationToken) =>
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
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .WithRateLimit(
                    "get-room-by-id",
                    60,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Room.GetByIdRateLimited);

            group.MapPut(Router.RoomRoutes.Update, async (
                    Guid id,
                    UpdateRoomRequest request,
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
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .WithRateLimit(
                    "update-room",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Room.UpdateRateLimited);

            group.MapDelete(Router.RoomRoutes.Delete, async (
                    Guid id,
                    IRoomService roomService,
                    CancellationToken cancellationToken) =>
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
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .WithRateLimit(
                    "delete-room",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Room.DeleteRateLimited);

            group.MapPost(Router.RoomRoutes.AddImage, async (
                    Guid id,
                    [FromForm] UploadRoomImageRequest request,
                    IRoomService roomService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await roomService.InitiateUploadAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .DisableAntiforgery()
                .WithName("UploadRoomImage")
                .WithDisplayName("Upload Room Image")
                .WithSummary("Queues an image upload for a room")
                .WithDescription("""
                                 Accepts a JPG, PNG or WEBP image as multipart/form-data and queues it
                                 for background upload to cloud storage. The image is created with the
                                 Pending status and becomes Uploaded (or Failed) once processed.
                                 When no display order is given, the image is appended to the gallery.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Accepts<UploadRoomImageRequest>("multipart/form-data")
                .Produces<ApiResponse<RoomImageResponse>>(StatusCodes.Status202Accepted)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity)
                .WithRateLimit(
                    "upload-room-image",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Room.UploadImageRateLimited);

            group.MapGet(Router.RoomRoutes.GetImages, async (
                    Guid id,
                    IRoomService roomService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await roomService.GetImagesAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("GetRoomImages")
                .WithDisplayName("Get Room Images")
                .WithSummary("Lists all images of a room with their upload status")
                .WithDescription("""
                                 Returns every image of the room, including Pending and Failed ones,
                                 ordered by display order. Use it to track background uploads.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces<ApiResponse<IReadOnlyList<RoomImageResponse>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .WithRateLimit(
                    "get-room-images",
                    60,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Room.GetImagesRateLimited);

            group.MapDelete(Router.RoomRoutes.DeleteImage, async (
                    Guid id,
                    Guid imageId,
                    IRoomService roomService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await roomService.DeleteImageAsync(id, imageId, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("DeleteRoomImage")
                .WithDisplayName("Delete Room Image")
                .WithSummary("Deletes an image from a room's gallery")
                .WithDescription("""
                                 Removes the image from the room's gallery immediately and queues
                                 the removal of the stored file from cloud storage.
                                 Returns a not found response when the image does not belong to the room.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .WithRateLimit(
                    "delete-room-image",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Room.DeleteImageRateLimited);
        }
    }
}