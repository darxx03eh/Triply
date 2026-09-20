using Microsoft.EntityFrameworkCore;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Domain.Enums.HotleImages;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the cart repository.</summary>
/// <summary>Persistence operations for cart.</summary>
public class CartRepository(TriplyDbContext context) : GenericRepository<CartItem>(context),
    ICartRepository
{
    /// <summary>Gets the user's cart.</summary>
    public async Task<List<CartItem>> GetUserCartAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.CartItems
            .Include(c => c.Room)
            .ThenInclude(r => r.Hotel)
            .ThenInclude(h => h.City)
            .Include(c => c.Room)
            .ThenInclude(r => r.Hotel)
            .ThenInclude(h => h.Images
                .Where(i => i.Status == HotelImageStatus.Uploaded && i.Url != null)
                .OrderBy(i => i.DisplayOrder)
                .Take(1))
            .Where(c => c.UserId == userId && !c.Room.Hotel.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

    /// <summary>Checks whether the city item exists already in the cart.</summary>
    public async Task<bool> IsItemExistsAsync(Guid userId, Guid roomId, DateTime checkIn, DateTime checkOut,
        CancellationToken cancellationToken = default)
        => await context.CartItems.AnyAsync(c =>
            c.UserId == userId
            && c.RoomId == roomId
            && c.CheckIn < checkOut
            && c.CheckOut > checkIn, cancellationToken);
    /// <summary>Deletes all items in the user's cart.</summary>
    public async Task<int> ClearAsync(Guid userId, CancellationToken cancellationToken = default)
        => await context.CartItems
            .Where(c => c.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
}