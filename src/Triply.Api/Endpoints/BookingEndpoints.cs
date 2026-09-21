using Triply.Api.Extensions;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Bookings;
using Triply.Application.Features.Bookings.Commands.Checkout;
using Triply.Application.Features.Bookings.Queries.GetBookings;
using Triply.Application.Interfaces.Services;
using Triply.Infrastructure.Routes;

namespace Triply.Api.Endpoints;

/// <summary>Maps the cart endpoints.</summary>
public static class BookingEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the cart endpoints.</summary>
        public void MapBookingEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Bookings")
                .RequireAuthorization();

            group.MapPost(Router.BookingRoutes.Checkout, async (
                    CheckoutRequest request, ICurrentUserAccessor user, IBookingService bookingService,
                    CancellationToken cancellationToken) =>
                {
                    request.UserId = user.UserId;
                    var result = await bookingService.CheckoutAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("Checkout")
                .WithDisplayName("Checkout")
                .WithSummary("Books all the rooms in the cart")
                .WithDescription("""
                                 Turns the rooms in the signed-in user's cart into pending bookings that share
                                 one confirmation number, applies the running deals and empties the cart.
                                 The rooms are checked again inside a serializable transaction so the same room
                                 can not be booked twice for the same dates.
                                 Unpaid pending bookings are cancelled automatically after a while.
                                 """)
                .Produces<ApiResponse<BookingConfirmationResponse>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .ProducesValidationProblem();

            group.MapGet(Router.UserRoutes.Bookings, async (
                    [AsParameters] GetBookingsRequest request, ICurrentUserAccessor user,
                    IBookingService bookingService, CancellationToken cancellationToken) =>
                {
                    var result = await bookingService.GetUserBookingsAsync(user.UserId, request, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetMyBookings")
                .WithDisplayName("Get My Bookings")
                .WithSummary("Retrieves the signed-in user's bookings")
                .WithDescription("""
                                 Retrieves a paginated list of the signed-in user's bookings, newest first,
                                 supports filtering and sorting (filters=Status==Confirmed, sorts=CheckIn).
                                 """)
                .Produces<ApiResponse<PagedResult<BookingResponse>>>(StatusCodes.Status200OK);

            group.MapGet(Router.BookingRoutes.GetByConfirmationNumber, async (
                    string confirmationNumber, ICurrentUserAccessor user, IBookingService bookingService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bookingService.GetByConfirmationNumberAsync(
                        confirmationNumber, user.UserId, user.IsAdmin, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetBookingByConfirmationNumber")
                .WithDisplayName("Get Booking by confirmation number")
                .WithSummary("Retrieves a booking by its confirmation number")
                .WithDescription("""
                                 Retrieves the guest details, rooms, dates and prices of a booking using its
                                 confirmation number. Users can see their own bookings only.
                                 """)
                .Produces<ApiResponse<BookingConfirmationResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapPost(Router.BookingRoutes.Cancel, async (
                    string confirmationNumber, ICurrentUserAccessor user, IBookingService bookingService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bookingService.CancelAsync(
                        confirmationNumber, user.UserId, user.IsAdmin, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("CancelBooking")
                .WithDisplayName("Cancel Booking")
                .WithSummary("Cancels a booking")
                .WithDescription("""
                                 Cancels all the rooms of a booking before the check-in date.
                                 Users can cancel their own bookings and administrators can cancel any booking.
                                 """)
                .Produces<ApiResponse<BookingConfirmationResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest);
        }
    }
}