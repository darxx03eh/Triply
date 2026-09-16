using Triply.Domain.Entities.Identity;

namespace Triply.Domain.Entities;

public sealed class UserRecentVisit
{
    public UserRecentVisit()
    {
        VisitId =  Guid.NewGuid();
        VisitedAt = DateTime.UtcNow;
    }
    public Guid VisitId { get; set; }

    public Guid UserId { get; set; }

    public Guid HotelId { get; set; }

    public DateTime VisitedAt { get; set; }

    public TriplyUser User { get; set; } = null!;

    public Hotel Hotel { get; set; } = null!;
}