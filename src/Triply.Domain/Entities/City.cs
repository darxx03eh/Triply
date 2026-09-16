using Triply.Domain.Entities.Base;

namespace Triply.Domain.Entities;

public sealed class City : BaseEntity
{
    public City()
    {
        CityId = Guid.NewGuid();
        Hotels = new HashSet<Hotel>();
        IsDeleted = false;
    }
    public Guid CityId { get; set; }
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? PostOffice  { get; set; }
    public bool IsDeleted { get; set; }
    public byte[] RowVersion { get; set; } = null!;
    public ICollection<Hotel> Hotels { get; set; }
}