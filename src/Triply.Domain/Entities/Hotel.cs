using Triply.Domain.Entities.Base;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Enums.Hotels;

namespace Triply.Domain.Entities;

public sealed class Hotel : BaseEntity
{
    public Hotel()
    {
        HotelId = Guid.NewGuid();
        Rooms = new HashSet<Room>();
        Images = new HashSet<HotelImage>();
        RecentVisits = new HashSet<UserRecentVisit>();
        HotelAmenities = new HashSet<HotelAmenities>();
        IsDeleted = false;
    }
    public Guid HotelId { get; set; }
    public string Name { get; set; } = null!;

    public Guid CityId { get; set; }

    public Guid? OwnerId { get; set; }

    public byte StarRating { get; set; }

    public HotelType HotelType { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsDeleted { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public City City { get; set; } = null!;

    public TriplyUser? Owner { get; set; }

    public ICollection<Room> Rooms { get; set; }
    public ICollection<HotelImage> Images { get; set; }
    public ICollection<UserRecentVisit> RecentVisits { get; set; }
    public ICollection<HotelAmenities> HotelAmenities { get; set; }
}