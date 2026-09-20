using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Configurations;

/// <summary>Entity Framework configuration of the user recent visit entity.</summary>
public class UserRecentVisitConfigurations : IEntityTypeConfiguration<UserRecentVisit>
{
    /// <summary>
    /// Apply User Recent Visit Configurations
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<UserRecentVisit> builder)
    {
        builder.ToTable("UserRecentVisits")
            .HasQueryFilter(x => !x.Hotel.IsDeleted);

        builder.HasKey(x => x.VisitId);

        builder.HasIndex(x => new { x.UserId, x.VisitedAt })
            .IsDescending(false, true)
            .HasDatabaseName("IX_UserRecentVisits_UserId");

        builder.HasIndex(x => new { x.UserId, x.HotelId })
            .IsUnique()
            .HasDatabaseName("UQ_UserRecentVisits_User_Hotel");

        builder.HasIndex(x => x.HotelId)
            .HasDatabaseName("IX_UserRecentVisits_HotelId");

        builder.HasOne(x => x.User)
            .WithMany(x => x.RecentVisits)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_UserRecentVisits_Users");
        
        builder.HasOne(x => x.Hotel)
            .WithMany(x => x.RecentVisits)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_UserRecentVisits_Hotels");
        
        builder.Property(x => x.VisitedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
    }
}