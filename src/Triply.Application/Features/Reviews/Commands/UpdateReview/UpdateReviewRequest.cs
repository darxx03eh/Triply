namespace Triply.Application.Features.Reviews.Commands.UpdateReview;

/// <summary>Request payload used to update review.</summary>
public class UpdateReviewRequest
{
    /// <summary>Gets or sets the rating.</summary>
    public byte Rating { get; set; }
    /// <summary>Gets or sets the title.</summary>
    public string? Title { get; set; }
    /// <summary>Gets or sets the comment.</summary>
    public string Comment { get; set; }
}