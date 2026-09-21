using Microsoft.Extensions.Logging;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Bookings;
using Triply.Application.Extensions;
using Triply.Application.Features.Bookings.Queries.GetBookings;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Bookings;

public partial class BookingService
{
    /// <summary>Gets bookings for specific user by his identifier.</summary>
    public async Task<Result<PagedResult<BookingResponse>>> GetUserBookingsAsync(Guid userId, 
        GetBookingsRequest request,
        CancellationToken cancellationToken = default)
    {
        var (bookings, totalCount) = await bookingRepository
            .GetUserBookingsAsync(userId, request, cancellationToken);

        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 10;

        var pagedResult = new PagedResult<BookingResponse>
        {
            Items = bookings.Select(b => b.ToBookingResponse()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        logger.LogDebug(
            "Bookings of user {UserId} retrieved: {ItemCount} items on page {Page} (page size {PageSize}," +
            " total {TotalCount})",
            userId, bookings.Count, page, pageSize, totalCount);

        return Result<PagedResult<BookingResponse>>.Success(pagedResult, success: bookings.Count == 0
            ? new("BOOKINGS_EMPTY", "You do not have any bookings yet.")
            : new("BOOKINGS_FOUND", "Your bookings were retrieved successfully."));
    }
}