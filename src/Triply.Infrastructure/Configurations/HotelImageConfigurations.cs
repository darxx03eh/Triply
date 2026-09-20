using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;
using Triply.Domain.Enums.HotleImages;

namespace Triply.Infrastructure.Configurations;

/// <summary>Entity Framework configuration of the hotel image entity.</summary>
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

            var statuses = string.Join(", ", Enum.GetNames<HotelImageStatus>().Select(status => $"'{status}'"));
            x.HasCheckConstraint("CK_Images_Status",
                $"[{nameof(HotelImage.Status)}] IN ({statuses})");
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
            .IsRequired(false);

        builder.Property(x => x.PublicId)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();
    }
}