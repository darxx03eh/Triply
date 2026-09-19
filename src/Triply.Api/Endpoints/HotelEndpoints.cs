using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Triply.Api.Extensions;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Application.Features.Hotels.Commands.UpdateHotel;
using Triply.Application.Features.Hotels.Commands.UploadImage;
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
                })
                .WithName("GetHotelById")
                .WithDisplayName("Get Hotel by id")
                .WithSummary("Retrieves a hotel by its ID")
                .WithDescription("""
                                 Retrieves the details of a specific hotel using its unique 
                                 identifier: gallery, amenities, location and review summary.
                                 This endpoint is public and powers the hotel page.
                                 """)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut(Router.HotelRoutes.Update, async (
                    Guid id, UpdateHotelRequest request, 
                    IHotelService hotelService, 
                    CancellationToken cancellationToken) =>
                {
                    request.HotelId = id;
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
            
            group.MapPost(Router.HotelRoutes.AddImage, async (
                    Guid id, [FromForm] UploadHotelImageRequest request,
                    IHotelService hotelService, CancellationToken cancellationToken) =>
                {
                    var result = await hotelService.InitiateUploadAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .DisableAntiforgery()
                .WithName("UploadHotelImage")
                .WithDisplayName("Upload Hotel Image")
                .WithSummary("Queues an image upload for a hotel")
                .WithDescription("""
                                 Accepts a JPG, PNG or WEBP image as multipart/form-data and queues it
                                 for background upload to cloud storage. The image is created with the
                                 Pending status and becomes Uploaded (or Failed) once processed.
                                 When no display order is given, the image is appended to the gallery.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Accepts<UploadHotelImageRequest>("multipart/form-data")
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status404NotFound)
                .ProducesValidationProblem();

            group.MapGet(Router.HotelRoutes.GetImages, async (
                    Guid id, IHotelService hotelService, CancellationToken cancellationToken) =>
                {
                    var result = await hotelService.GetImagesAsync(id, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("GetHotelImages")
                .WithDisplayName("Get Hotel Images")
                .WithSummary("Lists all images of a hotel with their upload status")
                .WithDescription("""
                                 Returns every image of the hotel, including Pending and Failed ones,
                                 ordered by display order. Use it to track background uploads.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete(Router.HotelRoutes.DeleteImage, async (
                    Guid id, Guid imageId, IHotelService hotelService, CancellationToken cancellationToken) =>
                {
                    var result = await hotelService.DeleteImageAsync(id, imageId, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
                .WithName("DeleteHotelImage")
                .WithDisplayName("Delete Hotel Image")
                .WithSummary("Deletes an image from a hotel's gallery")
                .WithDescription("""
                                 Removes the image from the hotel's gallery immediately and queues
                                 the removal of the stored file from cloud storage.
                                 Returns a not found response when the image does not belong to the hotel.
                                 This endpoint is restricted to users with the Admin role.
                                 """)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }
    }
}