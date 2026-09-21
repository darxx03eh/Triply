using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Filtering;

/// <summary>Gets or sets the application Sieve processor.</summary>
/// <summary>Represents the application Sieve processor.</summary>
public class ApplicationSieveProcessor(IOptions<SieveOptions> options) : SieveProcessor(options)
{
    /// <summary>Maps the properties.</summary>
    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        // Maps the city properties.
        mapper.Property<City>(c => c.Name).CanFilter().CanSort();
        mapper.Property<City>(c => c.Country).CanFilter().CanSort();
        mapper.Property<City>(c => c.CreatedAt).CanSort();
        mapper.Property<City>(c => c.ModifiedAt).CanSort();
        
        // Maps the hotel properties.
        mapper.Property<Hotel>(h => h.Name).CanFilter().CanSort();
        mapper.Property<Hotel>(h => h.StarRating).CanFilter().CanSort();
        mapper.Property<Hotel>(h => h.CityId).CanFilter();
        mapper.Property<Hotel>(h => h.HotelType)
            .CanFilter().CanSort()
            .HasName("type");
        mapper.Property<Hotel>(h => h.CreatedAt).CanSort();
        mapper.Property<Hotel>(h => h.ModifiedAt).CanSort();

        // Maps the room properties.
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

        // Maps the review properties.
        mapper.Property<Review>(r => r.Rating)
            .CanFilter().CanSort()
            .HasName("rate");
        mapper.Property<Review>(r => r.CreatedAt).CanSort();
        
        // Maps the deal properties.
        mapper.Property<Deal>(d => d.RoomId).CanFilter();
        mapper.Property<Deal>(d => d.Title).CanFilter().CanSort();
        mapper.Property<Deal>(d => d.DiscountPercentage)
            .CanFilter().CanSort()
            .HasName("discount");
        mapper.Property<Deal>(d => d.IsFeatured)
            .CanFilter()
            .HasName("featured");
        mapper.Property<Deal>(d => d.StartsAt).CanFilter().CanSort();
        mapper.Property<Deal>(d => d.EndsAt).CanFilter().CanSort();
        mapper.Property<Deal>(d => d.CreatedAt).CanSort();

        // Maps the booking properties.
        mapper.Property<Booking>(b => b.Status).CanFilter().CanSort();
        mapper.Property<Booking>(b => b.CheckIn).CanFilter().CanSort();
        mapper.Property<Booking>(b => b.CheckOut).CanFilter().CanSort();
        mapper.Property<Booking>(b => b.TotalPrice)
            .CanFilter().CanSort()
            .HasName("price");
        mapper.Property<Booking>(b => b.CreatedAt).CanSort();

        return mapper;
    }
}