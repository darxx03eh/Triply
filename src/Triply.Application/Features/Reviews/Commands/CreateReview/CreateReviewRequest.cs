using System.Text.Json.Serialization;

namespace Triply.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewRequest
{
    [JsonIgnore]
    public Guid HotelId { get; set; }
    [JsonIgnore]
    public Guid UserId { get; set; }
    public byte Rating { get; set; }
    public string? Title { get; set; }
    public string Comment { get; set; }
}