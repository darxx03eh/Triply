namespace Triply.Application.Features.Reviews.Commands.UpdateReview;

public class UpdateReviewRequest
{
    public byte Rating { get; set; }
    public string? Title { get; set; }
    public string Comment { get; set; }
}