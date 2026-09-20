using Triply.Domain.Entities.Base;
using Triply.Domain.Entities.Identity;

namespace Triply.Domain.Entities;

/// <summary>Represents the cart item.</summary>
public class CartItem : BaseEntity
{
    // <summary>Initializes a new instance of the cart item.</summary>
    public CartItem()
    {
        CartItemId = Guid.NewGuid();
        Adults = 2;
    }
    /// <summary>Gets or sets the identifier of the cart item.</summary>
    public Guid CartItemId { get; set; }
    /// <summary>Gets or sets the identifier of the user.</summary>
    public Guid UserId { get; set; }
    /// <summary>Gets or sets the identifier of the room.</summary>
    public Guid RoomId { get; set; }
    /// <summary>Gets or sets the check in.</summary>
    public DateTime CheckIn { get; set; }
    /// <summary>Gets or sets the check-out.</summary>
    public DateTime CheckOut { get; set; }
    /// <summary>Gets or sets the adults.</summary>
    public short Adults { get; set; } 
    /// <summary>Gets or sets the children.</summary>
    public short Children { get; set; }
    /// <summary>Gets or sets the navigation property for user.</summary>
    public TriplyUser User { get; set; }
    /// <summary>Gets or sets the navigation property for room.</summary>
    public Room Room { get; set;  }
}