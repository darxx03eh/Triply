namespace Triply.Application.DTOs.Deals;

public record DealResponse()
{
    public Guid DealId { get; init; }
    public Guid RoomId { get; init; }
    public string Title { get; init; }
    public decimal DiscountPercentage { get; init; }
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public bool IsFeatured { get; init; }
    public DateTime CreatedAt { get; init; }
}