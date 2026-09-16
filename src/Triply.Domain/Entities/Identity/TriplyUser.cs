using Microsoft.AspNetCore.Identity;

namespace Triply.Domain.Entities.Identity;

public sealed class TriplyUser : IdentityUser<Guid>
{
    public TriplyUser()
    {
        RefreshTokens = new HashSet<RefreshToken>();
        OwnedHotels = new HashSet<Hotel>();
        Bookings = new HashSet<Booking>();
        RecentVisits = new HashSet<UserRecentVisit>();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
        IsActive = true;
    }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public ICollection<Hotel> OwnedHotels { get; set; }
    public ICollection<Booking> Bookings { get; set; }
    public ICollection<UserRecentVisit> RecentVisits { get; set; }
}