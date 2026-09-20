using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Configurations;

/// <summary>Entity Framework configuration of the amenity entity.</summary>
public class AmenityConfigurations : IEntityTypeConfiguration<Amenity>
{
    /// <summary>
    /// Apply Amenity Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.ToTable("Amenities");
        
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_Amenities_Name");

        builder.HasKey(x => x.AmenityId);
        
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
    }
}