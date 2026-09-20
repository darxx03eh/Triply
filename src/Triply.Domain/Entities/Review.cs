using Triply.Domain.Entities.Base;
using Triply.Domain.Entities.Identity;

namespace Triply.Domain.Entities;

/// <summary>Represents the review.</summary>
public class Review : BaseEntity
{
    /// <summary>Initializes a new instance of the review.</summary>
    public Review() => ReviewId = Guid.NewGuid();

    /// <summary>Gets or sets the identifier of the review.</summary>
    public Guid ReviewId { get; set; }
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    public Guid HotelId { get; set; }
    /// <summary>Gets or sets the identifier of the user.</summary>
    public Guid UserId { get; set; }
    /// <summary>Gets or sets the rating.</summary>
    public byte Rating { get; set; }
    /// <summary>Gets or sets the title.</summary>
    public string? Title { get; set; }
    /// <summary>Gets or sets the comment.</summary>
    public string Comment { get; set; } = null!;
    /// <summary>Gets or sets the navigation property for hotel.</summary>
    public Hotel Hotel { get; set; } = null!;
    /// <summary>Gets or sets the navigation property for user.</summary>
    public TriplyUser User { get; set; } = null!;
}