using Triply.Domain.Entities.Base;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Enums.Bookings;

namespace Triply.Domain.Entities;

public sealed class Booking : BaseEntity
{
    public Booking()
    {
        BookingId = Guid.NewGuid();
        Status = BookingStatus.Pending;
    }
    public Guid BookingId { get; set; }
    
    public Guid UserId { get; set; }

    public Guid RoomId { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    public short Adults { get; set; } = 2;

    public short Children { get; set; }

    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; }

    public string? SpecialRequests { get; set; }

    public TriplyUser User { get; set; } = null!;

    public Room Room { get; set; } = null!;

    public Payment? Payment { get; set; }
}