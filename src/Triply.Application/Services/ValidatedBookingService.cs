using FluentValidation;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Bookings;
using Triply.Application.Extensions;
using Triply.Application.Features.Bookings.Commands.Checkout;
using Triply.Application.Features.Bookings.Queries.GetBookings;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

/// <summary>Gets or sets the validated booking service.</summary>
/// <summary>Validates the requests before delegating to the booking service.</summary>
public class ValidatedBookingService(
    IBookingService inner,
    IEnumerable<IValidator<CheckoutRequest>> checkoutValidators,
    IEnumerable<IValidator<GetBookingsRequest>> getBookingsValidators) : IBookingService
{
    /// <summary>Checkout a booking.</summary>
    public async Task<Result<BookingConfirmationResponse>> CheckoutAsync(CheckoutRequest request,
        CancellationToken cancellationToken = default)
    {
        await checkoutValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CheckoutAsync(request, cancellationToken);
    }

    /// <summary>Get the user's bookings.</summary>
    public async Task<Result<PagedResult<BookingResponse>>> GetUserBookingsAsync(Guid userId,
        GetBookingsRequest request, CancellationToken cancellationToken = default)
    {
        await getBookingsValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.GetUserBookingsAsync(userId, request, cancellationToken);
    }

    /// <summary>Get a booking by confirmation number.</summary>
    public async Task<Result<BookingConfirmationResponse>> GetByConfirmationNumberAsync(string confirmationNumber,
        Guid userId, bool isAdmin, CancellationToken cancellationToken = default)
        => await inner.GetByConfirmationNumberAsync(confirmationNumber, userId, isAdmin, cancellationToken);

    /// <summary>Cancel a booking.</summary>
    public async Task<Result<BookingConfirmationResponse>> CancelAsync(string confirmationNumber, Guid userId,
        bool isAdmin, CancellationToken cancellationToken = default)
        => await inner.CancelAsync(confirmationNumber, userId, isAdmin, cancellationToken);
}