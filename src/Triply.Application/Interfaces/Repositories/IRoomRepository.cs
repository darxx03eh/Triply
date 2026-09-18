using Sieve.Models;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

public interface IRoomRepository : IGenericRepository<Room>
{
    Task<Room?> GetByIdWithHotelAsync(Guid roomId, CancellationToken cancellationToken = default);

    Task<(List<Room> Rooms, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        bool isAdmin,
        Guid? hotelId = null,
        CancellationToken cancellationToken = default);

    Task<bool> IsRoomNumberExistsAsync(Guid hotelId, string number, CancellationToken cancellationToken = default);

    Task<bool> IsRoomNumberExistsExcludeIdAsync(string number, Guid roomId,
        CancellationToken cancellationToken = default);
    void SetOriginalRowVersion(Room room, byte[] rowVersion);
}
