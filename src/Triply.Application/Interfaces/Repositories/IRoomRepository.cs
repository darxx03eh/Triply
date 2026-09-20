using Sieve.Models;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for room.</summary>
public interface IRoomRepository : IGenericRepository<Room>
{
    /// <summary>Gets the room by its identifier including the hotel.</summary>
    Task<Room?> GetByIdWithHotelAsync(Guid roomId, CancellationToken cancellationToken = default);

    /// <summary>Gets a paginated list of rooms.</summary>
    Task<(List<Room> Rooms, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        bool isAdmin,
        Guid? hotelId = null,
        CancellationToken cancellationToken = default);

    /// <summary>Checks whether the room number exists.</summary>
    Task<bool> IsRoomNumberExistsAsync(Guid hotelId, string number, CancellationToken cancellationToken = default);

    /// <summary>Checks whether the room number exists exclude identifier.</summary>
    Task<bool> IsRoomNumberExistsExcludeIdAsync(string number, Guid roomId,
        CancellationToken cancellationToken = default);
    /// <summary>Sets the original row version.</summary>
    void SetOriginalRowVersion(Room room, byte[] rowVersion);
}
