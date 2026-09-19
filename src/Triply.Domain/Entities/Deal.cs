using Triply.Domain.Entities.Base;

namespace Triply.Domain.Entities;

public class Deal : BaseEntity
{
    public Deal()
    {
        DealId = Guid.NewGuid();
        IsFeatured = true;
    }
    public Guid DealId { get; set; }

    public Guid RoomId { get; set; }

    public string Title { get; set; } = null!;

    public decimal DiscountPercentage { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public bool IsFeatured { get; set; }

    public Room Room { get; set; } = null!;
}