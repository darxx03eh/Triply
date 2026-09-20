using Triply.Domain.Entities.Base;

namespace Triply.Domain.Entities;

/// <summary>Represents the city.</summary>
public sealed class City : BaseEntity
{
    /// <summary>Initializes a new instance of the city.</summary>
    public City()
    {
        CityId = Guid.NewGuid();
        Hotels = new HashSet<Hotel>();
        IsDeleted = false;
    }
    /// <summary>Gets or sets the identifier of the city.</summary>
    public Guid CityId { get; set; }
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; } = null!;
    /// <summary>Gets or sets the country.</summary>
    public string Country { get; set; } = null!;
    /// <summary>Gets or sets the post office.</summary>
    public string? PostOffice  { get; set; }
    /// <summary>Gets or sets the thumbnail URL.</summary>
    public string? ThumbnailUrl { get; set; }
    /// <summary>Gets or sets the identifier of the thumbnail public.</summary>
    public string? ThumbnailPublicId { get; set; }
    /// <summary>Gets or sets whether the city is deleted.</summary>
    public bool IsDeleted { get; set; }
    /// <summary>Gets or sets the row version.</summary>
    public byte[] RowVersion { get; set; } = null!;
    /// <summary>Gets or sets the navigation property for hotels.</summary>
    public ICollection<Hotel> Hotels { get; set; }
}