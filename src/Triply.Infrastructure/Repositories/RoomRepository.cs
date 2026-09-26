using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Enums.Images;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the room repository.</summary>
/// <summary>Persistence operations for room.</summary>
public class RoomRepository(TriplyDbContext context, ISieveProcessor sieveProcessor)
    : GenericRepository<Room>(context),
    IRoomRepository
{
    /// <summary>Gets the room by its identifier including the hotel.</summary>
    public async Task<Room?> GetByIdWithHotelAndImagesAsync(Guid roomId, CancellationToken cancellationToken = default)
        => await context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Images.Where(i => i.Status == ImageStatus.Uploaded && i.Url != null)
                .OrderBy(i => i.DisplayOrder))
            .FirstOrDefaultAsync(r => r.RoomId == roomId, cancellationToken);

    /// <summary>Gets a paginated list of rooms.</summary>
    public async Task<(List<Room> Rooms, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        bool isAdmin,
        Guid? hotelId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Images.Where(i => i.Status == ImageStatus.Uploaded && i.Url != null)
                .OrderBy(i => i.DisplayOrder))
            .AsQueryable();

        if (isAdmin) query = query.IgnoreQueryFilters();
        if (hotelId.HasValue) query = query.Where(r => r.HotelId == hotelId.Value);

        query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var totalCount = await query.CountAsync(cancellationToken);

        query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
        var rooms = await query.ToListAsync(cancellationToken);

        return (rooms, totalCount);
    }

    /// <summary>Checks whether the room number exists.</summary>
    public async Task<bool> IsRoomNumberExistsAsync(Guid hotelId, string number,
        CancellationToken cancellationToken = default)
        => await context.Rooms.AnyAsync(r =>
            r.HotelId == hotelId && r.Number.ToUpper() == number.ToUpper(), cancellationToken);

    /// <summary>Checks whether the room number exists exclude identifier.</summary>
    public async Task<bool> IsRoomNumberExistsExcludeIdAsync(string number, Guid roomId,
        CancellationToken cancellationToken = default)
        => await context.Rooms.AnyAsync(r =>
                r.Number.ToUpper() == number.ToUpper()
                && r.RoomId != roomId
                && r.HotelId == context.Rooms
                    .Where(x => x.RoomId == roomId)
                    .Select(x => x.HotelId)
                    .FirstOrDefault(),
            cancellationToken);

    /// <summary>Checks whether a room has a pending or confirmed booking that overlaps the requested stay.</summary>
    public Task<bool> IsBookedAsync(Guid roomId, DateTime checkIn, DateTime checkOut,
        CancellationToken cancellationToken = default)
        => context.Bookings.AnyAsync(booking =>
            booking.RoomId == roomId &&
            (booking.Status == BookingStatus.Pending || booking.Status == BookingStatus.Confirmed) &&
            booking.CheckIn < checkOut && booking.CheckOut > checkIn,
            cancellationToken);

    /// <summary>Sets the original row version.</summary>
    public void SetOriginalRowVersion(Room room, byte[] rowVersion)
        => context.Entry(room).Property(r => r.RowVersion).OriginalValue = rowVersion;
}
