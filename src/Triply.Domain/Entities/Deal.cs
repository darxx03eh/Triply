using Triply.Domain.Entities.Base;

namespace Triply.Domain.Entities;

/// <summary>Represents the deal.</summary>
public class Deal : BaseEntity
{
    /// <summary>Initializes a new instance of the deal.</summary>
    public Deal()
    {
        DealId = Guid.NewGuid();
        IsFeatured = true;
    }
    /// <summary>Gets or sets the identifier of the deal.</summary>
    public Guid DealId { get; set; }

    /// <summary>Gets or sets the identifier of the room.</summary>
    public Guid RoomId { get; set; }

    /// <summary>Gets or sets the title.</summary>
    public string Title { get; set; } = null!;

    /// <summary>Gets or sets the discount percentage.</summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>Gets or sets when the deal starts.</summary>
    public DateTime StartsAt { get; set; }

    /// <summary>Gets or sets when the deal ends.</summary>
    public DateTime EndsAt { get; set; }

    /// <summary>Gets or sets whether the deal is featured.</summary>
    public bool IsFeatured { get; set; }

    /// <summary>Gets or sets the navigation property for room.</summary>
    public Room Room { get; set; } = null!;
}