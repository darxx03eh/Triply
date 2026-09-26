using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Images;

namespace Triply.Infrastructure.Configurations;

/// <summary>Entity Framework configuration of the room image entity.</summary>
public class RoomImageConfigurations : IEntityTypeConfiguration<RoomImage>
{
    /// <summary>
    /// Apply Room Image Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<RoomImage> builder)
    {
        builder.ToTable("RoomImages", x =>
        {
            x.HasCheckConstraint("CK_DisplayOrder_Positive",
                $"[{nameof(RoomImage.DisplayOrder)}] > 0");

            var statuses = string.Join(", ", Enum.GetNames<ImageStatus>().Select(status => $"'{status}'"));
            x.HasCheckConstraint("CK_Images_Status",
                $"[{nameof(RoomImage.Status)}] IN ({statuses})");
        });

        builder.HasIndex(x => x.RoomId)
            .HasDatabaseName("IX_RoomImages_RoomId");

        builder.HasKey(x => x.ImageId);

        builder.HasOne(x => x.Room)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RoomImages_Rooms");

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
