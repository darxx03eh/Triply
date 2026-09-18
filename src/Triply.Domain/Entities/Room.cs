using Triply.Domain.Entities.Base;
using Triply.Domain.Enums.Rooms;

namespace Triply.Domain.Entities;

public sealed class Room : BaseEntity
{
    public Room()
    {
        RoomId =  Guid.NewGuid();
        Bookings =  new HashSet<Booking>();
        AdultCapacity = 2;
        IsAvailable = true;
        IsDeleted = false;
    }
    public Guid RoomId { get; set; }
    public Guid HotelId { get; set; }

    public string Number { get; set; } = null!;

    public RoomType RoomType { get; set; }

    public short AdultCapacity { get; set; }

    public short ChildCapacity { get; set; }

    public decimal PricePerNight { get; set; }

    public string? Description { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsDeleted { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public Hotel Hotel { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; }
}