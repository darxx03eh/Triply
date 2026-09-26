using Triply.Domain.Entities.Base;
using Triply.Domain.Enums.Rooms;

namespace Triply.Domain.Entities;

/// <summary>Represents the room.</summary>
public sealed class Room : BaseEntity
{
    /// <summary>Initializes a new instance of the room.</summary>
    public Room()
    {
        RoomId =  Guid.NewGuid();
        Bookings =  new HashSet<Booking>();
        Deals = new HashSet<Deal>();
        CartItems = new HashSet<CartItem>();
        Images = new HashSet<RoomImage>();
        AdultCapacity = 2;
        IsAvailable = true;
        IsDeleted = false;
    }
    /// <summary>Gets or sets the identifier of the room.</summary>
    public Guid RoomId { get; set; }
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    public Guid HotelId { get; set; }

    /// <summary>Gets or sets the number.</summary>
    public string Number { get; set; } = null!;

    /// <summary>Gets or sets the room type.</summary>
    public RoomType RoomType { get; set; }

    /// <summary>Gets or sets the adult capacity.</summary>
    public short AdultCapacity { get; set; }

    /// <summary>Gets or sets the child capacity.</summary>
    public short ChildCapacity { get; set; }

    /// <summary>Gets or sets the price per night.</summary>
    public decimal PricePerNight { get; set; }

    /// <summary>Gets or sets the description.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets whether the room is available.</summary>
    public bool IsAvailable { get; set; }

    /// <summary>Gets or sets whether the room is deleted.</summary>
    public bool IsDeleted { get; set; }

    /// <summary>Gets or sets the row version.</summary>
    public byte[] RowVersion { get; set; } = null!;

    /// <summary>Gets or sets the navigation property for hotel.</summary>
    public Hotel Hotel { get; set; } = null!;

    /// <summary>Gets or sets the navigation property for bookings.</summary>
    public ICollection<Booking> Bookings { get; set; }
    /// <summary>Gets or sets the navigation property for deals.</summary>
    public ICollection<Deal> Deals { get; set; }
    /// <summary>Get or sets the navigation property for cart items.</summary>
    public ICollection<CartItem> CartItems { get; set; }
    /// <summary>Gets or sets the navigation property for images.</summary>
    public ICollection<RoomImage> Images { get; set; }
}
