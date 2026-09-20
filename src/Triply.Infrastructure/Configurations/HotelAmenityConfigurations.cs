using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Configurations;

/// <summary>Entity Framework configuration of the hotel amenity entity.</summary>
public class HotelAmenityConfigurations : IEntityTypeConfiguration<HotelAmenities>
{
    /// <summary>
    /// Apply Hotel Amenities Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<HotelAmenities> builder)
    {
        builder.ToTable("HotelAmenities");

        builder.HasKey(x => x.HotelAmenitiesId);

        builder.HasIndex(x => new { x.HotelId, x.AmenityId })
            .IsUnique()
            .HasDatabaseName("UQ_HotelAmenities_Hotel_Amenity");
        
        builder.HasOne(x => x.Hotel)
            .WithMany(x => x.HotelAmenities)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_HotelAmenities_Hotel");
        
        builder.HasOne(x => x.Amenity)
            .WithMany(x => x.HotelAmenities)
            .HasForeignKey(x => x.AmenityId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_HotelAmenities_Amenity");
    }
}