using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Configurations;

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