namespace Triply.Domain.Entities.Base;

/// <summary>Provides audit timestamps shared by persisted entities.</summary>
public class BaseEntity
{
    /// <summary>Gets or sets the creation time in UTC.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>Gets or sets the last modification time in UTC.</summary>
    public DateTime? ModifiedAt { get; set; }
}