using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for room image.</summary>
public interface IRoomImageRepository : IGenericRepository<RoomImage>
{
    /// <summary>Gets the next display order.</summary>
    Task<short> GetNextDisplayOrderAsync(Guid roomId, CancellationToken cancellationToken = default);
    /// <summary>Gets the hotel image by its room identifier.</summary>
    Task<List<RoomImage>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);
}