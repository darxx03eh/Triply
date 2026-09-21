using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Bookings;
using Triply.Application.Features.Bookings.Commands.Checkout;
using Triply.Application.Features.Bookings.Queries.GetBookings;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Results;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the booking repository.</summary>
/// <summary>Persistence operations for booking.</summary>
public class BookingRepository(TriplyDbContext context, ISieveProcessor sieveProcessor) 
    : GenericRepository<Booking>(context),
    IBookingRepository
{
    /// <summary>Checks whether the room is already booked during the specified date range.</summary>
    public async Task<bool> IsRoomBookedAsync(Guid roomId, DateTime checkIn, DateTime checkOut,
        CancellationToken cancellationToken = default)
        => await context.Bookings.AnyAsync(b =>
            b.RoomId == roomId
            && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
            && b.CheckIn < checkOut
            && b.CheckOut > checkIn, cancellationToken);

    /// <summary>Check if the confirmation number exists or not.</summary>
    public async Task<bool> IsConfirmationNumberExistsAsync(string confirmationNumber,
        CancellationToken cancellationToken = default)
        => await context.Bookings.AnyAsync(b => b.ConfirmationNumber == confirmationNumber, cancellationToken);

    /// <summary>Gets the list of Booking by confirmation number.</summary>
    public async Task<List<Booking>> GetByConfirmationNumberAsync(string confirmationNumber,
        CancellationToken cancellationToken = default)
        => await context.Bookings
            .IgnoreQueryFilters()
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .ThenInclude(h => h.City)
            .Where(b => b.ConfirmationNumber == confirmationNumber)
            .OrderBy(b => b.CheckIn)
            .ToListAsync(cancellationToken);

    /// <summary>Gets a paged bookings and their counts for specific user by his identifier.</summary>
    public async Task<(List<Booking> Bookings, int TotalCount)> GetUserBookingsAsync(Guid userId, SieveModel sieveModel,
        CancellationToken cancellationToken = default)
    {
        var query = context.Bookings
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .ThenInclude(h => h.City)
            .Where(b => b.UserId == userId);

        if (string.IsNullOrWhiteSpace(sieveModel.Sorts))
            query = query.OrderByDescending(b => b.CreatedAt);

        query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var totalCount = await query.CountAsync(cancellationToken);

        query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
        var bookings = await query.ToListAsync(cancellationToken);

        return (bookings, totalCount);
    }

    /// <summary>Expires pending bookings created before the specified date.</summary>
    public async Task<int> ExpirePendingAsync(DateTime createdBefore, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await context.Bookings
            .Where(b => b.Status == BookingStatus.Pending && b.CreatedAt < createdBefore)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(b => b.Status, BookingStatus.Cancelled)
                .SetProperty(b => b.CanceledAt, now)
                .SetProperty(b => b.ModifiedAt, now), cancellationToken);
    }

    /// <summary>Checks if there is a completed booking for user in specific hotel.</summary>
    public async Task<bool> HasCompletedBookingAtHotelAsync(Guid userId, Guid hotelId,
        CancellationToken cancellationToken = default)
        => await context.Bookings
            .AsNoTracking()
            .AnyAsync(booking =>
                    booking.UserId == userId &&
                    booking.Status == BookingStatus.Completed &&
                    booking.Room.HotelId == hotelId,
                cancellationToken);
}