using Triply.Domain.Entities.Base;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Enums.Bookings;

namespace Triply.Domain.Entities;

/// <summary>Represents the booking.</summary>
public sealed class Booking : BaseEntity
{
    /// <summary>Initializes a new instance of the booking.</summary>
    public Booking()
    {
        BookingId = Guid.NewGuid();
        Status = BookingStatus.Pending;
    }
    /// <summary>Gets or sets the identifier of the booking.</summary>
    public Guid BookingId { get; set; }
    
    /// <summary>Gets or sets the identifier of the user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the identifier of the room.</summary>
    public Guid RoomId { get; set; }

    /// <summary>Gets or sets the check in.</summary>
    public DateTime CheckIn { get; set; }

    /// <summary>Gets or sets the check out.</summary>
    public DateTime CheckOut { get; set; }

    /// <summary>Gets or sets the adults.</summary>
    public short Adults { get; set; } = 2;

    /// <summary>Gets or sets the children.</summary>
    public short Children { get; set; }

    /// <summary>Gets or sets the total price.</summary>
    public decimal TotalPrice { get; set; }

    /// <summary>Gets or sets the status.</summary>
    public BookingStatus Status { get; set; }

    /// <summary>Gets or sets the special requests.</summary>
    public string? SpecialRequests { get; set; }

    /// <summary>Gets or sets the navigation property for user.</summary>
    public TriplyUser User { get; set; } = null!;

    /// <summary>Gets or sets the navigation property for room.</summary>
    public Room Room { get; set; } = null!;

    /// <summary>Gets or sets the navigation property for payment.</summary>
    public Payment? Payment { get; set; }
}