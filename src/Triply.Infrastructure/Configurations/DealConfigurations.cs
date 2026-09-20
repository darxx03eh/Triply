using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Configurations;

/// <summary>Entity Framework configuration of the deal entity.</summary>
public class DealConfigurations : IEntityTypeConfiguration<Deal>
{
    /// <summary>
    /// Apply Deal Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("Deals", x =>
        {
            x.HasCheckConstraint("CK_Deals_Discount",
                $"[{nameof(Deal.DiscountPercentage)}] > 0 AND [{nameof(Deal.DiscountPercentage)}] <= 90");

            x.HasCheckConstraint("CK_Deals_Date",
                $"[{nameof(Deal.EndsAt)}] > [{nameof(Deal.StartsAt)}]");
        }).HasQueryFilter(x => !x.Room.IsDeleted);
        
        builder.HasKey(x => x.DealId);

        builder.HasIndex(x => new { x.RoomId, x.StartsAt, x.EndsAt })
            .HasDatabaseName("IX_Deals_RoomId_Dates");

        builder.HasIndex(x => new { x.IsFeatured, x.EndsAt })
            .HasDatabaseName("IX_Deals_IsFeatured_EndsAt");

        builder.HasOne(x => x.Room)
            .WithMany(x => x.Deals)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Deals_Rooms");
        
        builder.Property(x => x.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DiscountPercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.IsFeatured)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
    }
}