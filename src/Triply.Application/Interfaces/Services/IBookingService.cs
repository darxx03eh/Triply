using Triply.Application.Common.Models;
using Triply.Application.DTOs.Bookings;
using Triply.Application.Features.Bookings.Commands.Checkout;
using Triply.Application.Features.Bookings.Queries.GetBookings;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the booking operations.</summary>
public interface IBookingService
{
    /// <summary>Checkout a booking operation.</summary>
    Task<Result<BookingConfirmationResponse>> CheckoutAsync(CheckoutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Get the user's bookings operation.</summary>
    Task<Result<PagedResult<BookingResponse>>> GetUserBookingsAsync(Guid userId, GetBookingsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Get a booking by confirmation number operation.</summary>
    Task<Result<BookingConfirmationResponse>> GetByConfirmationNumberAsync(string confirmationNumber, Guid userId,
        bool isAdmin, CancellationToken cancellationToken = default);

    /// <summary>Cancel a booking operation.</summary>
    Task<Result<BookingConfirmationResponse>> CancelAsync(string confirmationNumber, Guid userId, bool isAdmin,
        CancellationToken cancellationToken = default);
}