using Triply.Domain.Entities.Base;
using Triply.Domain.Entities.Identity;

namespace Triply.Domain.Entities;

public class Review : BaseEntity
{
    public Review() => ReviewId = Guid.NewGuid();

    public Guid ReviewId { get; set; }
    public Guid HotelId { get; set; }
    public Guid UserId { get; set; }
    public byte Rating { get; set; }
    public string? Title { get; set; }
    public string Comment { get; set; } = null!;
    public Hotel Hotel { get; set; } = null!;
    public TriplyUser User { get; set; } = null!;
}