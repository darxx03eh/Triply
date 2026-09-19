using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Filtering;

public class ApplicationSieveProcessor(IOptions<SieveOptions> options) : SieveProcessor(options)
{
    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        mapper.Property<City>(c => c.Name).CanFilter().CanSort();
        mapper.Property<City>(c => c.Country).CanFilter().CanSort();
        mapper.Property<City>(c => c.CreatedAt).CanSort();
        mapper.Property<City>(c => c.ModifiedAt).CanSort();
        
        mapper.Property<Hotel>(h => h.Name).CanFilter().CanSort();
        mapper.Property<Hotel>(h => h.StarRating).CanFilter().CanSort();
        mapper.Property<Hotel>(h => h.CityId).CanFilter();
        mapper.Property<Hotel>(h => h.HotelType)
            .CanFilter().CanSort()
            .HasName("type");
        mapper.Property<Hotel>(h => h.CreatedAt).CanSort();
        mapper.Property<Hotel>(h => h.ModifiedAt).CanSort();

        mapper.Property<Room>(r => r.Number).CanFilter().CanSort();
        mapper.Property<Room>(r => r.HotelId).CanFilter();
        mapper.Property<Room>(r => r.RoomType)
            .CanFilter().CanSort()
            .HasName("room");
        mapper.Property<Room>(r => r.AdultCapacity)
            .CanFilter().CanSort()
            .HasName("adults");
        mapper.Property<Room>(r => r.ChildCapacity)
            .CanFilter().CanSort()
            .HasName("children");
        mapper.Property<Room>(r => r.PricePerNight)
            .CanFilter().CanSort()
            .HasName("price");
        mapper.Property<Room>(r => r.IsAvailable)
            .CanFilter()
            .HasName("available");
        mapper.Property<Room>(r => r.CreatedAt).CanSort();
        mapper.Property<Room>(r => r.ModifiedAt).CanSort();

        mapper.Property<Review>(r => r.Rating)
            .CanFilter().CanSort()
            .HasName("rate");
        mapper.Property<Review>(r => r.CreatedAt).CanSort();

        return mapper;
    }
}