using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Configurations;

public class AttractionConfigurations : IEntityTypeConfiguration<Attraction>
{
    /// <summary>
    /// Apply Attraction Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Attraction> builder)
    {
        builder.ToTable("Attractions", x =>
        {
            x.HasCheckConstraint("CK_Attractions_Distance",
                $"[{nameof(Attraction.DistanceKm)}] >= 0");
        }).HasQueryFilter(x => !x.Hotel.IsDeleted);
        
        builder.HasKey(x => x.AttractionId);

        builder.HasIndex(x => x.HotelId)
            .HasDatabaseName("IX_Attractions_HotelId");
        
        builder.HasOne(x => x.Hotel)
            .WithMany(x => x.Attractions)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Attractions_Hotels");
        
        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Category)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DistanceKm)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
    }
}