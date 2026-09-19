using Microsoft.AspNetCore.Identity;

namespace Triply.Domain.Entities.Identity;

/// <summary>Represents an application user.</summary>
public sealed class TriplyUser : IdentityUser<Guid>
{
    /// <summary>Initializes a new active user with empty navigation collections.</summary>
    public TriplyUser()
    {
        RefreshTokens = new HashSet<RefreshToken>();
        OwnedHotels = new HashSet<Hotel>();
        Bookings = new HashSet<Booking>();
        RecentVisits = new HashSet<UserRecentVisit>();
        Reviews = new HashSet<Review>();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
        IsActive = true;
    }
    /// <summary>Gets or sets the user's first name.</summary>
    public string FirstName { get; set; }
    /// <summary>Gets or sets the user's last name.</summary>
    public string LastName { get; set; }
    /// <summary>Gets or sets the user's date of birth.</summary>
    public DateTime? DateOfBirth { get; set; }
    /// <summary>Gets or sets when the user was created.</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Gets or sets when the user was last modified.</summary>
    public DateTime? ModifiedAt { get; set; }
    /// <summary>Gets or sets the last successful login time.</summary>
    public DateTime? LastLoginAt { get; set; }
    /// <summary>Gets or sets whether the user is deleted.</summary>
    public bool IsDeleted { get; set; }
    /// <summary>Gets or sets whether the user is active.</summary>
    public bool IsActive { get; set; }
    /// <summary>Gets or sets the user's refresh tokens.</summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    /// <summary>Gets or sets hotels owned by the user.</summary>
    public ICollection<Hotel> OwnedHotels { get; set; }
    /// <summary>Gets or sets bookings made by the user.</summary>
    public ICollection<Booking> Bookings { get; set; }
    /// <summary>Gets or sets the user's recent hotel visits.</summary>
    public ICollection<UserRecentVisit> RecentVisits { get; set; }
    /// <summary>Gets or sets the reviews written by the user.</summary>
    public ICollection<Review> Reviews { get; set; }
}