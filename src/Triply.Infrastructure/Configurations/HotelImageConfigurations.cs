using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Configurations;

public class HotelImageConfigurations : IEntityTypeConfiguration<HotelImage>
{
    /// <summary>
    /// Apply Hotel Image Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<HotelImage> builder)
    {
        builder.ToTable("HotelImages", x =>
        {
            x.HasCheckConstraint("CK_DisplayOrder_Positive",
                $"[{nameof(HotelImage.DisplayOrder)}] > 0");
        });
        
        builder.HasIndex(x => x.HotelId)
            .HasDatabaseName("IX_HotelImages_HotelId");

        builder.HasKey(x => x.ImageId);
        
        builder.HasOne(x => x.Hotel)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_HotelImages_Hotels");
        
        builder.Property(x => x.Url)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired();
    }
}