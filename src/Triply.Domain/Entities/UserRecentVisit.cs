using Triply.Domain.Entities.Identity;

namespace Triply.Domain.Entities;

/// <summary>Represents the user recent visit.</summary>
public sealed class UserRecentVisit
{
    /// <summary>Initializes a new instance of the user recent visit.</summary>
    public UserRecentVisit()
    {
        VisitId =  Guid.NewGuid();
        VisitedAt = DateTime.UtcNow;
    }
    /// <summary>Gets or sets the identifier of the visit.</summary>
    public Guid VisitId { get; set; }

    /// <summary>Gets or sets the identifier of the user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the identifier of the hotel.</summary>
    public Guid HotelId { get; set; }

    /// <summary>Gets or sets when the user recent visit was visited.</summary>
    public DateTime VisitedAt { get; set; }

    /// <summary>Gets or sets the navigation property for user.</summary>
    public TriplyUser User { get; set; } = null!;

    /// <summary>Gets or sets the navigation property for hotel.</summary>
    public Hotel Hotel { get; set; } = null!;
}