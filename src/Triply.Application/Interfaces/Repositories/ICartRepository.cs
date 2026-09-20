using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for cart.</summary>
public interface ICartRepository : IGenericRepository<CartItem>
{
    /// <summary>Gets the cart for specific user by his identifier.</summary>
    Task<List<CartItem>> GetUserCartAsync(Guid userId, CancellationToken cancellationToken = default);
    /// <summary>Check if the item already exists in user's cart.</summary>
    Task<bool> IsItemExistsAsync(Guid userId, Guid roomId, DateTime checkIn, DateTime checkOut,
        CancellationToken cancellationToken = default);
    /// <summary>Clear the cart for specific user by his identifier.</summary>
    Task<int> ClearAsync(Guid userId, CancellationToken cancellationToken = default);
}