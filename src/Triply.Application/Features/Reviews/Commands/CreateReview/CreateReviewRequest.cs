using System.Text.Json.Serialization;

namespace Triply.Application.Features.Reviews.Commands.CreateReview;

/// <summary>Request payload used to create review.</summary>
public class CreateReviewRequest
{
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    [JsonIgnore]
    public Guid HotelId { get; set; }
    /// <summary>Gets or sets the identifier of the user.</summary>
    [JsonIgnore]
    public Guid UserId { get; set; }
    /// <summary>Gets or sets the rating.</summary>
    public byte Rating { get; set; }
    /// <summary>Gets or sets the title.</summary>
    public string? Title { get; set; }
    /// <summary>Gets or sets the comment.</summary>
    public string Comment { get; set; }
}