using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Configurations;

public class ReviewConfigurations : IEntityTypeConfiguration<Review>
{
    /// <summary>
    /// Apply Review Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews", x =>
        {
            x.HasCheckConstraint("CK_Reviews_Rating",
                $"[{nameof(Review.Rating)}] BETWEEN 1 AND 5");
        }).HasQueryFilter(x => !x.Hotel.IsDeleted);
        
        builder.HasKey(x => x.ReviewId);

        builder.HasIndex(x => new { x.HotelId, x.UserId })
            .IsUnique()
            .HasDatabaseName("UQ_Reviews_Hotel_User");

        builder.HasIndex(x => new { x.HotelId, x.CreatedAt })
            .HasDatabaseName("IX_Reviews_HotelId_CreatedAt");

        builder.HasOne(x => x.Hotel)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Reviews_Hotels");
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Reviews_Users");

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(100);

        builder.Property(x => x.Comment)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
    }
}