namespace Triply.Application.DTOs.Reviews;

public record ReviewResponse()
{
    public Guid ReviewId { get; init; }
    public Guid HotelId { get; init; }
    public Guid UserId { get; init; }
    public string AuthorName { get; init; }
    public byte Rating { get; init; }
    public string? Title { get; init; }
    public string Comment { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}