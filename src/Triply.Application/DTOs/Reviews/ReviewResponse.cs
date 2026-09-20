namespace Triply.Application.DTOs.Reviews;

/// <summary>Gets or sets the review response.</summary>
/// <summary>Response returned for the review.</summary>
public record ReviewResponse()
{
    /// <summary>Gets the identifier of the review.</summary>
    public Guid ReviewId { get; init; }
    /// <summary>Gets the identifier of the hotel.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the identifier of the user.</summary>
    public Guid UserId { get; init; }
    /// <summary>Gets the author name.</summary>
    public string AuthorName { get; init; }
    /// <summary>Gets the rating.</summary>
    public byte Rating { get; init; }
    /// <summary>Gets the title.</summary>
    public string? Title { get; init; }
    /// <summary>Gets the comment.</summary>
    public string Comment { get; init; }
    /// <summary>Gets when the review was created.</summary>
    public DateTime CreatedAt { get; init; }
    /// <summary>Gets when the review was modified.</summary>
    public DateTime? ModifiedAt { get; init; }
}