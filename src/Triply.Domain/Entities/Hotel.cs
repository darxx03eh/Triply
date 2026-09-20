using Triply.Domain.Entities.Base;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Enums.Hotels;

namespace Triply.Domain.Entities;

/// <summary>Represents the hotel.</summary>
public sealed class Hotel : BaseEntity
{
    /// <summary>Initializes a new instance of the hotel.</summary>
    public Hotel()
    {
        HotelId = Guid.NewGuid();
        Rooms = new HashSet<Room>();
        Images = new HashSet<HotelImage>();
        RecentVisits = new HashSet<UserRecentVisit>();
        HotelAmenities = new HashSet<HotelAmenities>();
        Attractions = new HashSet<Attraction>();
        Reviews = new HashSet<Review>();
        IsDeleted = false;
    }
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    public Guid HotelId { get; set; }
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Gets or sets the identifier of the city.</summary>
    public Guid CityId { get; set; }

    /// <summary>Gets or sets the identifier of the owner.</summary>
    public Guid? OwnerId { get; set; }

    /// <summary>Gets or sets the star rating.</summary>
    public byte StarRating { get; set; }

    /// <summary>Gets or sets the hotel type.</summary>
    public HotelType HotelType { get; set; }

    /// <summary>Gets or sets the address.</summary>
    public string? Address { get; set; }

    /// <summary>Gets or sets the description.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the latitude.</summary>
    public decimal? Latitude { get; set; }

    /// <summary>Gets or sets the longitude.</summary>
    public decimal? Longitude { get; set; }

    /// <summary>Gets or sets whether the hotel is deleted.</summary>
    public bool IsDeleted { get; set; }

    /// <summary>Gets or sets the row version.</summary>
    public byte[] RowVersion { get; set; } = null!;

    /// <summary>Gets or sets the navigation property for city.</summary>
    public City City { get; set; } = null!;

    /// <summary>Gets or sets the navigation property for owner.</summary>
    public TriplyUser? Owner { get; set; }

    /// <summary>Gets or sets the rooms.</summary>
    public ICollection<Room> Rooms { get; set; }
    /// <summary>Gets or sets the navigation property for images.</summary>
    public ICollection<HotelImage> Images { get; set; }
    /// <summary>Gets or sets the navigation property for recent visits.</summary>
    public ICollection<UserRecentVisit> RecentVisits { get; set; }
    /// <summary>Gets or sets the navigation property for hotel amenities.</summary>
    public ICollection<HotelAmenities> HotelAmenities { get; set; }
    /// <summary>Gets or sets the navigation property for attractions.</summary>
    public ICollection<Attraction> Attractions { get; set; }
    /// <summary>Gets or sets the navigation property for reviews.</summary>
    public ICollection<Review> Reviews { get; set; }
}