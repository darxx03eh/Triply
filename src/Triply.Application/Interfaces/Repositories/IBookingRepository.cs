using Sieve.Models;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for booking.</summary>
public interface IBookingRepository : IGenericRepository<Booking>
{
    /// <summary>Checks whether the room is already booked during the specified date range.</summary>
    Task<bool> IsRoomBookedAsync(Guid roomId, DateTime checkIn, DateTime checkOut,
        CancellationToken cancellationToken = default);
    /// <summary>Check if the confirmation number exists or not.</summary>
    Task<bool> IsConfirmationNumberExistsAsync(string confirmationNumber,
        CancellationToken cancellationToken = default);
    /// <summary>Gets the list of Booking by confirmation number.</summary>
    Task<List<Booking>> GetByConfirmationNumberAsync(string confirmationNumber,
        CancellationToken cancellationToken = default);
    /// <summary>Gets a paged bookings and their counts for specific user by his identifier.</summary>
    Task<(List<Booking> Bookings, int TotalCount)> GetUserBookingsAsync(Guid userId, SieveModel sieveModel,
        CancellationToken cancellationToken = default);
    /// <summary>Expires pending bookings created before the specified date.</summary>
    Task<int> ExpirePendingAsync(DateTime createdBefore, CancellationToken cancellationToken = default);
    /// <summary>Checks if there is a completed bookings in specific hotel for user, allow him to make a review.</summary>
    
    Task<bool> HasCompletedBookingAtHotelAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken = default);
}